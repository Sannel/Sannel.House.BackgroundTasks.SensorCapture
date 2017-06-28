using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sannel.House.Sensor;

namespace Sannel.House.SensorCapture.Data.Models
{
	public class SensorEntry
	{
		[Key]
		public Guid LocalId { get; set; }

		public SensorTypes SensorType { get; set; }

		public int DeviceId { get; set; }

		public double Value { get; set; }

		public double? Value2 { get; set; }
		public double? Value3 { get; set; }
		public double? Value4 { get; set; }
		public double? Value5 { get; set; }
		public double? Value6 { get; set; }
		public double? Value7 { get; set; }
		public double? Value8 { get; set; }
		public double? Value9 { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public override string ToString()
		{
			return $"LocalId {LocalId}, SensorType {SensorType}, DeviceId {DeviceId}, Value = {Value}, CreatedDate = {CreatedDate}";
		}
	}
}
