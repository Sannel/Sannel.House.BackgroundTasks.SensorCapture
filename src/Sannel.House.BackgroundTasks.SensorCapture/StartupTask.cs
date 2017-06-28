using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Sannel.House.SensorCapture.Data;
using Microsoft.EntityFrameworkCore;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Sannel.House.BackgroundTasks.SensorCapture
{
    public sealed class StartupTask : IBackgroundTask
    {
		private BackgroundTaskDeferral deferral;
		private ThreadPoolTimer timer;
		private BroadcastListener listener;

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

			var db = new DataContext();
			db.Database.Migrate();

			listener = new BroadcastListener(db);
			listener.Begin();

        }
    }
}
