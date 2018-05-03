using Newtonsoft.Json;
using Sannel.House.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.SensorCapture.Data
{
	public static class Extensions
	{
		public static string ConvertToString(this IDictionary<string, float> dict)
		{
			return JsonConvert.SerializeObject(dict);
		}
	}
}
