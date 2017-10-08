using Sannel.House.Sensor;
using Sannel.House.SensorCapture.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal static class Extensions
	{
		public static double? CheckForNaN(this double d)
		{
			if (double.IsNaN(d))
			{
				return null;
			}

			return d;
		}

		public static SensorEntry ToSensorEntry(this SensorPacket packet, long macAddress)
		{
			var entry = new SensorEntry()
			{
				MacAddress = macAddress,
				LocalId = Guid.NewGuid(),
				SensorType = packet.SensorType,
				Value = packet.Values[0],
				Value2 = packet.Values[1].CheckForNaN(),
				Value3 = packet.Values[2].CheckForNaN(),
				Value4 = packet.Values[3].CheckForNaN(),
				Value5 = packet.Values[4].CheckForNaN(),
				Value6 = packet.Values[5].CheckForNaN(),
				Value7 = packet.Values[6].CheckForNaN(),
				Value8 = packet.Values[7].CheckForNaN(),
				Value9 = packet.Values[8].CheckForNaN(),
				Value10 = packet.Values[9].CheckForNaN()
			};
			return entry;
		}
	}
}
