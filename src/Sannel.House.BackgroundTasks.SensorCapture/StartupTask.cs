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

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Sannel.House.BackgroundTasks.SensorCapture
{
    public sealed class StartupTask : IBackgroundTask
    {
		private BackgroundTaskDeferral deferral;
		private ThreadPoolTimer timer;
		private SensorPacketListener listener;
		private DataContext dataContext;

        public void Run(IBackgroundTaskInstance taskInstance)
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

			listener = new SensorPacketListener();
			listener.PacketReceived += packageReceived;
			listener.Begin(8172);

        }

		private async void packageReceived(object sender, SensorPacketReceivedEventArgs e)
		{
			await dataContext.SensorEntries.AddAsync(e.Packet.ToSensorEntry());
			await dataContext.SaveChangesAsync();
		}
	}
}
