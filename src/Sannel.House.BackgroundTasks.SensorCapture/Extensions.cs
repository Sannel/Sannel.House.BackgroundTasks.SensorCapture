using Sannel.House.Sensor;
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
	}
}
