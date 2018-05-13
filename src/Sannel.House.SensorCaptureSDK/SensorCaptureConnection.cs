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
using Newtonsoft.Json;
using Sannel.House.Sensor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace Sannel.House.SensorCaptureSDK
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public sealed class SensorCaptureConnection : IDisposable
	{
		private AppServiceConnection connection;

		/// <summary>
		/// Gets or sets the system identifier.
		/// </summary>
		/// <value>
		/// The system identifier.
		/// </value>
		public Guid SystemId
		{
			get;
			set;
		}

		public event TypedEventHandler<AppServiceConnection, AppServiceClosedEventArgs> ServiceClosed;

		/// <summary>
		/// Initializes a new instance of the <see cref="SensorCaptureConnection" /> class.
		/// </summary>
		public SensorCaptureConnection()
		{
			connection = new AppServiceConnection
			{
				PackageFamilyName = "Sannel.House.BackgroundTasks.SensorCapture_s8t1p3zkxq1s8",
				AppServiceName = "com.sannel.house.sensorcapture"
			};
			connection.ServiceClosed += connection_ServiceClosed;

			var di = new EasClientDeviceInformation();
			SystemId = di.Id;
		}

		private void connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args) 
			=> ServiceClosed?.Invoke(sender, args);

		/// <summary>
		/// Sends the temperature entry.
		/// </summary>
		/// <param name="temperature">The temperature.</param>
		/// <returns></returns>
		public IAsyncOperation<bool> SendTemperatureEntry(float temperature) 
			=> sendAsync(SensorHelper.Temperature.Create(temperature, SystemId))
			.AsAsyncOperation();

		/// <summary>
		/// Sends the THP.
		/// </summary>
		/// <param name="temperature">The temperature.</param>
		/// <param name="humidity">The humidity.</param>
		/// <param name="pressure">The pressure.</param>
		/// <returns></returns>
		public IAsyncOperation<bool> SendTHP(float temperature, float humidity, float pressure)
			=> sendAsync(SensorHelper.THP.Create(temperature, humidity, pressure, SystemId))
			.AsAsyncOperation();

		/// <summary>
		/// Sends an enumerable of entries
		/// </summary>
		/// <param name="entries">The entries.</param>
		/// <returns></returns>
		public IAsyncOperation<bool> SendEntriesAsync(IEnumerable<ISensorEntry> entries) 
			=> Task.Run(async () =>
			{
				var vs = new ValueSet();
				await Task.Run(() => vs[$"{nameof(SensorEntry)}s"] = JsonConvert.SerializeObject(entries));
				var result = await connection.SendMessageAsync(vs);
				return result.Status == AppServiceResponseStatus.Success;
			}).AsAsyncOperation();

		/// <summary>
		/// Sends the asynchronous.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <returns></returns>
		private async Task<bool> sendAsync(SensorEntry entry)
		{
			var vs = new ValueSet();
			await Task.Run(() => { vs[nameof(SensorEntry)] = JsonConvert.SerializeObject(entry); });
			var result = await connection.SendMessageAsync(vs);
			return result.Status == AppServiceResponseStatus.Success;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() 
			=> connection?.Dispose();

		/// <summary>
		/// Connects the asynchronous.
		/// </summary>
		/// <returns></returns>
		public IAsyncOperation<AppServiceConnectionStatus> ConnectAsync()
			=> Task.Run(async () =>
			{
				return await connection.OpenAsync();
			}).AsAsyncOperation();

	}
}
