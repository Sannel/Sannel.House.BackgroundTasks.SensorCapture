using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sannel.House.Sensor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.SensorCapture.Data
{
	public static class Extensions
	{
		public static Task AddAsync<T>(this DbSet<T> set, SensorEntry entry)
			where T : SensorEntry
		{
			return set.AddAsync(new LocalSensorEntry(entry));
		}
	}
}
