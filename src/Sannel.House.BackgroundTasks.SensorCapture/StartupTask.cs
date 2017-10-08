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
using Microsoft.Extensions.DependencyInjection;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral deferral;
		private ReadingsManager manager;

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

			var serviceCollection = new ServiceCollection();

			serviceCollection.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=SensorEntries.db"));
			serviceCollection.AddTransient<Configuration.ConfigurationConnection>();
			serviceCollection.AddTransient<TCPSensorPacketListener>();
			serviceCollection.AddTransient<ReadingsManager>();
			var provider = serviceCollection.BuildServiceProvider();

			manager = provider.GetService<ReadingsManager>();
			await manager.StartAsync();
		}

		private void onCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			manager?.Dispose();
			deferral?.Complete();
		}
	}
}
