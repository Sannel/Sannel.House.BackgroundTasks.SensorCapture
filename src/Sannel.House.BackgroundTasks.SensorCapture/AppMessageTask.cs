using Newtonsoft.Json;
using Sannel.House.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Mobile.Analytics;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Sannel.House.SensorCapture.Common;
using Sannel.House.SensorCapture.Data;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	public sealed class AppMessageTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;
		private DataContext context;

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			taskInstance.Canceled += this.canceled;
			deferral = taskInstance.GetDeferral();

			var v = Startup.Instance.Value;
			context = v.Provider.GetService<DataContext>();

			var td = taskInstance.TriggerDetails as AppServiceTriggerDetails;
			td.AppServiceConnection.RequestReceived += this.requestReceived;

		}

		private async void requestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
		{
			var messageDeferral = args.GetDeferral();
			try
			{
				var message = args.Request.Message;
				if(message.ContainsKey(nameof(SensorEntry)) && message[nameof(SensorEntry)] is string data)
				{
					var entry = await Task.Run(() => JsonConvert.DeserializeObject<SensorEntry>(data));
					if(entry != null)
					{
						entry.LocalId = Guid.NewGuid();
						await context.SensorEntries.AddAsync(entry);
						await context.SaveChangesAsync();
					}
				}
				else if(message.ContainsKey($"{nameof(SensorEntry)}s") && message[$"{nameof(SensorEntry)}s"] is string listString)
				{
					var entries = await Task.Run(() => JsonConvert.DeserializeObject<List<SensorEntry>>(listString));
					if(entries != null)
					{
						foreach(var e in entries)
						{
							e.LocalId = Guid.NewGuid();
							await context.SensorEntries.AddAsync(e);
						}
						await context.SaveChangesAsync();
					}
				}

			}
			catch(Exception ex)
			{

			}
			messageDeferral.Complete();
		}

		private void canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			deferral?.Complete();
		}
	}
}
