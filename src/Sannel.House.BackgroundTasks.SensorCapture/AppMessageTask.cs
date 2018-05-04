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
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Sannel.House.SensorCapture.Data;
using Microsoft.AppCenter.Analytics;
using Sannel.House.Configuration.Common;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	public sealed class AppMessageTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;
		private DataContext context;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			taskInstance.Canceled += this.canceled;
			deferral = taskInstance.GetDeferral();

			var v = await Startup.GetStartupAsync();
			Analytics.TrackEvent("AppMessageTask startup");
			context = v.Provider.GetService<DataContext>();

			var td = taskInstance.TriggerDetails as AppServiceTriggerDetails;
			td.AppServiceConnection.RequestReceived += this.requestReceived;

		}

		private async void requestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
		{
			Analytics.TrackEvent("AppMessage request Received");
			var messageDeferral = args.GetDeferral();
			try
			{
				var message = args.Request.Message;
				if(message.ContainsKey(nameof(SensorEntry)) && message[nameof(SensorEntry)] is string data)
				{
					Analytics.TrackEvent("AppMessage SensorEntry Received");
					var entry = await Task.Run(() => JsonConvert.DeserializeObject<SensorEntry>(data));
					if(entry != null)
					{
						entry.Id = Guid.NewGuid();
						await context.SensorEntries.AddAsync(new LocalSensorEntry(entry));
						await context.SaveChangesAsync();
					}
					else
					{
						Analytics.TrackEvent("AppMessage SensorEntry no valid entry received");
					}
				}
				else if(message.ContainsKey($"{nameof(SensorEntry)}s") && message[$"{nameof(SensorEntry)}s"] is string listString)
				{
					Analytics.TrackEvent("AppMessage SensorEntrys Received");
					var entries = await Task.Run(() => JsonConvert.DeserializeObject<List<SensorEntry>>(listString));
					if(entries != null)
					{
						foreach(var e in entries)
						{
							e.Id = Guid.NewGuid();
							await context.SensorEntries.AddAsync(e);
						}
						await context.SaveChangesAsync();
					}
					else
					{
						Analytics.TrackEvent("AppMessage SensorEntrys no valid entries received");
					}
				}
				else
				{
					Analytics.TrackEvent("AppMessage Unknown Request");
				}

			}
			catch(Exception ex)
			{
				ex.TrackEvent("AppMessage requestReceived Exception");
			}
			messageDeferral.Complete();
		}

		private void canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			Analytics.TrackEvent("AppMessage Task Canceled", new Dictionary<string, string>
			{
				{"Reason", reason.ToString() }
			});
			deferral?.Complete();
		}
	}
}
