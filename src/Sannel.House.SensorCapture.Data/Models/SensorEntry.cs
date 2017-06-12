using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.SensorCapture.Data.Models
{
	public class SensorEntry
	{
		[Key]
		public Guid LocalId { get; set; }

		public SensorType SensorType { get; set; }

		public int DeviceId { get; set; }

		public double Value { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public override string ToString()
		{
			return $"LocalId {LocalId}, SensorType {SensorType}, DeviceId {DeviceId}, Value = {Value}, CreatedDate = {CreatedDate}";
		}
	}
}
