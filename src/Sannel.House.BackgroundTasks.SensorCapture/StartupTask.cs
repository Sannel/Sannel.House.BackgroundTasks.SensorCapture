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
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Windows.Storage;
using Microsoft.AppCenter.Analytics;

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

			var v = await Startup.GetStartupAsync();

			Analytics.TrackEvent("BackgroundTask Started");

			manager = v.Provider.GetService<ReadingsManager>();
			manager.Start();
		}

		private void onCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			Analytics.TrackEvent("Background Task Canceled", new Dictionary<string, string>
			{
				{"Reason", reason.ToString() }
			});
			deferral?.Complete();
		}
	}
}
