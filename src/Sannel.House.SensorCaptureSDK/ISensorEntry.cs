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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.SensorCaptureSDK
{
	public interface ISensorEntry
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		Guid Id { get; }

		/// <summary>
		/// Gets or sets the type of the sensor.
		/// </summary>
		/// <value>
		/// The type of the sensor.
		/// </value>
		string SensorType { get; }

		/// <summary>
		/// Gets or sets the device identifier.
		/// </summary>
		/// <value>
		/// The device identifier.
		/// </value>
		int DeviceId { get; }

		/// <summary>
		/// Gets or sets the device mac address.
		/// </summary>
		/// <value>
		/// The device mac address.
		/// </value>
		long? DeviceMacAddress
		{
			get;
		}

		/// <summary>
		/// Gets or sets the device UUID.
		/// </summary>
		/// <value>
		/// The device UUID.
		/// </value>
		Guid? DeviceUuid
		{
			get;
		}

		/// <summary>
		/// Gets or sets the date created.
		/// </summary>
		/// <value>
		/// The date created.
		/// </value>
		[JsonProperty("DateCreated")]
		DateTimeOffset DateCreatedOffset { get; }

		/// <summary>
		/// Gets or sets the values.
		/// </summary>
		/// <value>
		/// The values.
		/// </value>
		IDictionary<string, float> Values { get; }

	}
}
