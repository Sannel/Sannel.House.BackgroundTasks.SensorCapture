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
		private TCPSensorPacketListener listener;
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
			taskInstance.Canceled += onCanceled;

			dataContext = new DataContext();
			dataContext.Database.Migrate();

#if DEBUG
			uint port = Defaults.Development.SENSOR_BROADCAST_PORT;
#else
			uint port = Defaults.Production.SENSOR_BROADCAST_PORT;
#endif
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
					port = (uint)(result["SensorsPort"] as int? ?? (int)port);
				}
			}

			listener = new TCPSensorPacketListener();
			listener.PacketReceived += packageReceived;
			listener.Begin(port);

			if (serverUri != null)
			{
				server = new ServerContext(serverUri);
				var (lresult, user) = await server.LoginAsync(username, password);
				if (lresult.Success)
				{
					timer = ThreadPoolTimer.CreateTimer(pushToServer, TimeSpan.FromMinutes(15));
				}
			}

			username = null;
			password = null;

		}

		private void onCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			dataContext?.Dispose();
			deferral?.Complete();
		}

		private async void pushToServer(ThreadPoolTimer timer)
		{
			if (server != null)
			{
				if (!server.IsAuthenticated)
				{
					await server.RefreshLoginAsync();
				}

				foreach (var entry in dataContext.SensorEntries.OrderBy(j => j.CreatedDate))
				{
					switch (entry.SensorType)
					{
						case SensorTypes.Temperature:
							var tEntry = new TemperatureEntry()
							{
								DateCreated = entry.CreatedDate,
								DeviceId = entry.DeviceId,
								Id = entry.LocalId,
								TemperatureCelsius = entry.Value,
								Pressure = 0,
								Humidity = 0
							};
							var addResult = await server.TemperatureEntries.PostAsync(tEntry);
							if (addResult.Success)
							{
								dataContext.SensorEntries.Remove(entry);
							}
							break;

						case SensorTypes.TemperatureHumidityPresure:
							var thpEntry = new TemperatureEntry()
							{
								DateCreated = entry.CreatedDate,
								DeviceId = entry.DeviceId,
								Id = entry.LocalId,
								TemperatureCelsius = entry.Value,
								Pressure = entry.Value2 ?? 0,
								Humidity = entry.Value3 ?? 0
							};
							var addTResult = await server.TemperatureEntries.PostAsync(thpEntry);
							if (addTResult.Success)
							{
								dataContext.SensorEntries.Remove(entry);
							}
							break;

						default:
							var sEntry = new SensorEntry()
							{
								DateCreated = entry.CreatedDate,
								DeviceId = entry.DeviceId,
								Id = entry.LocalId,
								SensorType = entry.SensorType,
								Value = entry.Value,
								Value2 = entry.Value2,
								Value3 = entry.Value3,
								Value4 = entry.Value4,
								Value5 = entry.Value5,
								Value6 = entry.Value6,
								Value7 = entry.Value7,
								Value8 = entry.Value8,
								Value9 = entry.Value9
							};
							var sResult = await server.SensorEntries.PostAsync(sEntry);
							if (sResult.Success)
							{
								dataContext.SensorEntries.Remove(entry);
							}
							break;
					}
				}
				await dataContext.SaveChangesAsync();
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
