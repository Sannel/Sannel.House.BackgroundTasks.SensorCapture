/* Copyright 2018 Sannel Software, L.L.C.

	Licensed under the Apache License, Version 2.0 (the ""License"");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an ""AS IS"" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.*/
using Microsoft.AppCenter.Analytics;
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
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Threading;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class ReadingsManager : IDisposable
	{
		private DataContext context;
		private Config config;
		private TCPSensorPacketListener listener;
		private ServerContext serverContext;
		private ThreadPoolTimer timer;
		private string username;
		private string password;

		public ReadingsManager(
			DataContext context,
			Config config,
			TCPSensorPacketListener listener)
		{
			this.context = context;
			this.context.Database.Migrate();
			this.config = config;
			this.listener = listener;
		}

		public void Start()
		{
			Analytics.TrackEvent("Configuration", new Dictionary<string, string>()
			{
				{"ServerApiUrl", config.ServiceApiUrl?.ToString() },
				{"Listen Port", config.Port.ToString() },
				{"Username", config.Username }
			});

			listener.EntriesReceived += this.entriesReceived;
			listener.Begin(config.Port);

			username = config.Username;
			password = config.Password;

			var di = new EasClientDeviceInformation();


			if (config.ServiceApiUrl != null)
			{
				serverContext = new ServerContext(config.ServiceApiUrl);
				pushToServer(null);
			}
		}

		private async void entriesReceived(object sender, SensorEntryReceivedEventArgs e)
			=> await ProcessEntriesAsync(e.MacAddress, e.Entries);

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
						Analytics.TrackEvent("Entry Type", new Dictionary<string, string>()
						{
							{"SensorType", entry?.SensorType.ToString() }
						});
						var status = await serverContext.SensorEntries.PostAsync(entry);
						if (status.Success)
						{
							context.SensorEntries.Remove(entry);
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


		public async Task ProcessEntriesAsync(long macAddress, IList<RemoteSensorEntry> entries)
		{
			try
			{
				Analytics.TrackEvent("Begin Receiving Packages");
				uint lastOffset = 0;
				var lastDateTime = DateTime.UtcNow;
				for(var i = entries.Count - 1; i >= 0; i--)
				{
					var p = entries[i];
					p.DeviceMacAddress = macAddress;

					if(lastOffset == 0)
					{
						p.DateCreated = lastDateTime;
					}
					else
					{
						var differnece = lastOffset - p.MillisOffset;
						lastOffset = p.MillisOffset;
						lastDateTime = lastDateTime.AddMilliseconds(differnece * -1);
						p.DateCreated = lastDateTime;
					}

					await context.SensorEntries.AddAsync(p);
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
			serverContext?.Dispose();
			context?.Dispose();
		}
	}
}
