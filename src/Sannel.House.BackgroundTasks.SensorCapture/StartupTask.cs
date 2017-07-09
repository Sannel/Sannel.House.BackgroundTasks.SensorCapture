using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Sannel.House.SensorCapture.Data;
using Microsoft.EntityFrameworkCore;
using Sannel.House.Sensor;
using Sannel.House.ServerSDK;
using Sannel.House.ServerSDK.Models;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;
		private ThreadPoolTimer timer;
		private SensorPacketListener listener;
		private DataContext dataContext;
		private ServerContext server;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			// 
			// TODO: Insert code to perform background work
			//
			// If you start any asynchronous methods here, prevent the task
			// from closing prematurely by using BackgroundTaskDeferral as
			// described in http://aka.ms/backgroundtaskdeferral
			//
			deferral = taskInstance.GetDeferral();

			dataContext = new DataContext();
			dataContext.Database.Migrate();

			uint port = 8175;
			Uri serverUri = null;
			string username = null;
			string password = null;

			using (var configuration = new Configuration.ConfigurationConnection())
			{
				if (await configuration.ConnectAsync())
				{
					var result = await configuration.GetConfiguration("ServerApiUrl", "ServerUsername", "ServerPassword", "SensorsPort");
					Uri.TryCreate(result["ServerApiUrl"] as string, UriKind.Absolute, out serverUri);
					username = result["ServerUsername"] as string;
					password = result["ServerPassword"] as string;
					port = (uint)(result["SensorsPort"] as int? ?? 8175);
				}
			}

			listener = new SensorPacketListener();
			listener.PacketReceived += packageReceived;
			listener.Begin(port);

			if (serverUri != null)
			{
				server = new ServerContext(serverUri);
				var (lresult, user) = await server.LoginAsync(username, password);
				if (lresult.Success)
				{
					timer = ThreadPoolTimer.CreateTimer(pushToServer, TimeSpan.FromMinutes(1));
				}
			}

			username = null;
			password = null;

		}

		private async void pushToServer(ThreadPoolTimer timer)
		{
			if (server != null)
			{
				if (!server.IsAuthenticated)
				{
					await server.RefreshLoginAsync();
				}

				var count = dataContext.SensorEntries.Count();
				var sCount = count - 10;

				for (var i = 0; i < count; i += 10)
				{
					sCount = count - i;
					if (sCount < 0)
					{
						sCount = 0;
					}

					foreach (var entry in dataContext.SensorEntries.OrderBy(j => j.CreatedDate).Skip(sCount).Take(10))
					{
						switch (entry.SensorType)
						{
							case SensorTypes.Temperature:
								var tEntry = new TemperatureEntry()
								{
									DateCreated = entry.CreatedDate,
									DeviceId = entry.DeviceId,
									Id = entry.LocalId,
									TemperatureCelsius = entry.Value
								};
								var addResult = await server.TemperatureEntries.PostAsync(tEntry);
								if (addResult.Success)
								{
									dataContext.SensorEntries.Remove(entry);
								}
								break;

							case SensorTypes.TemperatureHumidityPresure:
								var thpEntry = new TemperatureEntry();
								thpEntry.DateCreated = entry.CreatedDate;
								thpEntry.DeviceId = entry.DeviceId;
								thpEntry.Id = entry.LocalId;
								thpEntry.TemperatureCelsius = entry.Value;
								thpEntry.Pressure = entry.Value2 ?? 0;
								thpEntry.Humidity = entry.Value4 ?? 0;
								var addTResult = await server.TemperatureEntries.PostAsync(thpEntry);
								if (addTResult.Success)
								{
									dataContext.SensorEntries.Remove(entry);
								}
								break;
						}
					}
					await dataContext.SaveChangesAsync();
				}
			}
			this.timer = ThreadPoolTimer.CreateTimer(pushToServer, timer.Delay);
		}

		private async void packageReceived(object sender, SensorPacketReceivedEventArgs e)
		{
			await dataContext.SensorEntries.AddAsync(e.Packet.ToSensorEntry());
			await dataContext.SaveChangesAsync();
		}
	}
}
