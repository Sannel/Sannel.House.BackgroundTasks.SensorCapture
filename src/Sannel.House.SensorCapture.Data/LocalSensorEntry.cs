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
using System.Text;

namespace Sannel.House.SensorCapture.Data
{
	public class LocalSensorEntry : SensorEntry
	{
		public LocalSensorEntry()
		{
		}

		public LocalSensorEntry(SensorEntry entry)
		{
			this.Id = entry.Id;
			this.DateCreated = entry.DateCreated;
			this.DeviceId = entry.DeviceId;
			this.DeviceMacAddress = entry.DeviceMacAddress;
			this.DeviceUuid = entry.DeviceUuid;
			this.ExtraElements = entry.ExtraElements;
			this.SensorType = entry.SensorType;
			this.Values = entry.Values;
		}

		public static implicit operator LocalSensorEntry(RemoteSensorEntry entry)
			=> entry;

		public string SValues
		{
			get => JsonConvert.SerializeObject(Values);
			set => Values = JsonConvert.DeserializeObject<Dictionary<string, float>>(value);
		}
	}
}
