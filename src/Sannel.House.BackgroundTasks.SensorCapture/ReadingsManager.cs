using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
		private ILogger<ReadingsManager> logger;
		private string username;
		private string password;

		public ReadingsManager(
			DataContext context,
			ConfigurationConnection configuration,
			TCPSensorPacketListener listener,
			ILogger<ReadingsManager> logger
			)
		{
			this.context = context;
			this.context.Database.Migrate();
			this.connection = configuration;
			this.listener = listener;
			this.logger = logger;
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

			logger.LogDebug("ServerApiUrl {0}", serverUri);
			logger.LogDebug("Listen Port {0}", port);
			logger.LogDebug("Username {0}", username);
			logger.LogDebug("Password Length {0}", password?.Length);

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
			logger.LogInformation("Pushing to Server");
			if(serverContext != null)
			{
				try
				{
					if (!serverContext.IsAuthenticated)
					{
						logger.LogDebug("Not logged in to server. Attempting to refresh from token");
						var (status, user) = await serverContext.RefreshLoginAsync();
						if (!status.Success)
						{
							logger.LogDebug("Unable to refresh from token. Logging in with username and password");
							(status, user) = await serverContext.LoginAsync(username, password);
							if (status.Success)
							{
								logger.LogDebug("Logged in with user {0}", user);
							}
							else
							{
								logger.LogDebug("Unable to login");
							}
						}
					}

					foreach (var entry in context.SensorEntries)
					{
						switch (entry.SensorType)
						{
							case SensorTypes.Temperature:
								var tentry = new TemperatureEntry()
								{
									DeviceMacAddress = entry.MacAddress,
									DateCreated = entry.CreatedDate,
									TemperatureCelsius = entry.Value
								};
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
								var status3 = await serverContext.SensorEntries.PostAsync(sentry);
								if (status3.Success)
								{
									context.SensorEntries.Remove(entry);
								}
								break;
						}
					}

					await context.SaveChangesAsync();
				}
				catch
				{

				}
				finally
				{
					timer = ThreadPoolTimer.CreateTimer(pushToServer, TimeSpan.FromMinutes(15));
				}
			}
		}

		private void packagesReceived(object sender, SensorPacketsReceivedEventArgs e)
		{
			try
			{
				uint lastOffset = 0;
				var lastDateTime = DateTime.UtcNow;
				for(var i = e.Packets.Count - 1; i >= 0; i--)
				{
					var p = e.Packets[i];
					var entry = p.ToSensorEntry(e.MacAddress);

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

					context.SensorEntries.Add(entry);
				}
			}
			catch(Exception ex)
			{
			}
			finally
			{
				context.SaveChanges();
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
