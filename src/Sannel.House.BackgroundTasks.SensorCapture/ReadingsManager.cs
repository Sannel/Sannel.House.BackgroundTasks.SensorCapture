using Microsoft.Azure.Mobile.Analytics;
using Microsoft.EntityFrameworkCore;
using Sannel.House.Configuration;
using Sannel.House.Sensor;
using Sannel.House.SensorCapture.Data;
using Sannel.House.ServerSDK;
using Sannel.House.ServerSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class ReadingsManager : IDisposable
	{
		private DataContext context;
		private ConfigurationConnection connection;
		private TCPSensorPacketListener listener;
		private ServerContext serverContext;
		private ThreadPoolTimer timer;
		private string username;
		private string password;

		public ReadingsManager(
			DataContext context,
			ConfigurationConnection configuration,
			TCPSensorPacketListener listener)
		{
			this.context = context;
			this.context.Database.Migrate();
			this.connection = configuration;
			this.listener = listener;
		}

		public async Task StartAsync()
		{
#if DEBUG
			uint port = Defaults.Development.SENSOR_BROADCAST_PORT;
#else
			uint port = Defaults.Production.SENSOR_BROADCAST_PORT;
#endif
			Uri serverUri = null;
			username = null;
			password = null;

			using (connection)
			{
				if (await connection.ConnectAsync())
				{
					var result = await connection.GetConfiguration("ServerApiUrl", "ServerUsername", "ServerPassword", "SensorsPort");
					Uri.TryCreate(result["ServerApiUrl"] as string, UriKind.Absolute, out serverUri);
					username = result["ServerUsername"] as string;
					password = result["ServerPassword"] as string;
					port = (uint)(result["SensorsPort"] as int? ?? (int)port);
				}
			}

			connection = null;

			Analytics.TrackEvent("Configuration", new Dictionary<string, string>()
			{
				{"ServerApiUrl", serverUri.ToString() },
				{"Listen Port", port.ToString() },
				{"Username", username }
			});

			listener.PacketReceived += this.packagesReceived;
			listener.Begin(port);

			if (serverUri != null)
			{
				serverContext = new ServerContext(serverUri);
				pushToServer(null);
			}
		}
		private async void pushToServer(ThreadPoolTimer timer)
		{
			Analytics.TrackEvent("Start Push To Server");
			if(serverContext != null)
			{
				try
				{
					if (!serverContext.IsAuthenticated)
					{
						Analytics.TrackEvent("Not logged in to server. Attempting to refresh from token");
						var (status, user) = await serverContext.RefreshLoginAsync();
						if (!status.Success)
						{
							Analytics.TrackEvent("Unable to refresh from token. Logging in with username and password");
							(status, user) = await serverContext.LoginAsync(username, password);
							if (status.Success)
							{
								Analytics.TrackEvent("Logged in with user {0}", new Dictionary<string, string>()
								{
									{ "User", user.Name }
								});
							}
							else
							{
								Analytics.TrackEvent("Unable to login");
							}
						}
					}

					Analytics.TrackEvent("Sending Events to server",
					new Dictionary<string, string>()
					{
						{ "Count", $"{await context.SensorEntries.CountAsync()}" }
					});
					foreach (var entry in context.SensorEntries)
					{
						int? deviceId = null;
						if(entry.MacAddress == null && entry.Uuid != null)
						{
							var altResult = await serverContext.AlternateDeviceIds.GetFromSystemUuidAsync(entry.Uuid.Value);
							if (altResult.Success)
							{
								deviceId = altResult.Data.DeviceId;
							}
							else
							{
								continue; // errored out go to next result
							}
						}
						switch (entry.SensorType)
						{
							case SensorTypes.Temperature:
								var tentry = new TemperatureEntry()
								{
									DeviceMacAddress = entry.MacAddress,
									DateCreated = entry.CreatedDate,
									TemperatureCelsius = entry.Value
								};
								if (deviceId.HasValue)
								{
									tentry.DeviceId = deviceId.Value;
								}
								var status = await serverContext.TemperatureEntries.PostAsync(tentry);
								if (status.Success)
								{
									context.SensorEntries.Remove(entry);
								}
								break;
							case SensorTypes.TemperatureHumidityPresure:
								var tentry2 = new TemperatureEntry()
								{
									DeviceMacAddress = entry.MacAddress,
									DateCreated = entry.CreatedDate,
									TemperatureCelsius = entry.Value,
									Humidity = entry.Value2,
									Pressure = entry.Value3
								};
								if (deviceId.HasValue)
								{
									tentry2.DeviceId = deviceId.Value;
								}
								var status2 = await serverContext.TemperatureEntries.PostAsync(tentry2);
								if (status2.Success)
								{
									context.SensorEntries.Remove(entry);
								}
								break;
							default:
								var sentry = new SensorEntry()
								{
									DeviceMacAddress = entry.MacAddress,
									DateCreated = entry.CreatedDate,
									Value = entry.Value,
									Value2 = entry.Value2,
									Value3 = entry.Value3,
									Value4 = entry.Value4,
									Value5 = entry.Value5,
									Value6 = entry.Value6,
									Value7 = entry.Value7,
									Value8 = entry.Value8,
									Value9 = entry.Value9,
									Value10 = entry.Value10,
									SensorType = entry.SensorType
								};

								if (deviceId.HasValue)
								{
									sentry.DeviceId = deviceId.Value;
								}
								var status3 = await serverContext.SensorEntries.PostAsync(sentry);
								if (status3.Success)
								{
									context.SensorEntries.Remove(entry);
								}
								break;
						}
					}

					await context.SaveChangesAsync();
					Analytics.TrackEvent("Events sent to server");
				}
				catch(Exception ex)
				{
					Analytics.TrackEvent("Exception trying to send data to server",
						new Dictionary<string, string>()
						{
							{ "Exception", ex.ToString() }
						}
					);
				}
				finally
				{
					timer = ThreadPoolTimer.CreateTimer(pushToServer, TimeSpan.FromMinutes(15));
				}
			}
		}

		private async void packagesReceived(object sender, SensorPacketsReceivedEventArgs e) 
			=> await ProcessPackatesAsync(e.MacAddress, e.Packets);

		public async Task ProcessPackatesAsync(long macAddress, IList<SensorPacket> packets)
		{
			try
			{
				Analytics.TrackEvent("Begin Receiving Packages");
				uint lastOffset = 0;
				var lastDateTime = DateTime.UtcNow;
				for(var i = packets.Count - 1; i >= 0; i--)
				{
					var p = packets[i];
					var entry = p.ToSensorEntry(macAddress);

					if(lastOffset == 0)
					{
						entry.CreatedDate = lastDateTime;
					}
					else
					{
						var differnece = lastOffset - p.MillisOffset;
						lastOffset = p.MillisOffset;
						lastDateTime = lastDateTime.AddMilliseconds(differnece * -1);
						entry.CreatedDate = lastDateTime;
					}

					await context.SensorEntries.AddAsync(entry);
				}

				Analytics.TrackEvent("Finished Receiving Packages");
			}
			catch(Exception ex)
			{
				Analytics.TrackEvent("Exception in package capture", new Dictionary<string, string>()
				{
					{ "Exception", ex.ToString() }
				});
			}
			finally
			{
				await context.SaveChangesAsync();
			}
		}

		public void Dispose()
		{
			connection?.Dispose();
			serverContext?.Dispose();
			context?.Dispose();
		}
	}
}
