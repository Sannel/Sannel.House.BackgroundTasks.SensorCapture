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
