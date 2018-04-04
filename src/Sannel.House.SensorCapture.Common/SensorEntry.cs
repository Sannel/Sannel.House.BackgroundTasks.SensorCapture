using Sannel.House.Sensor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sannel.House.SensorCapture.Common
{
	/// <summary>
	/// An entry to store back to the database
	/// </summary>
	public class SensorEntry
	{
		/// <summary>
		/// Gets or sets the local identifier
		/// </summary>
		/// <value>
		/// The local identifier.
		/// </value>
		[Key]
		public Guid LocalId { get; set; }

		/// <summary>
		/// Gets or sets the type of the sensor.
		/// </summary>
		/// <value>
		/// The type of the sensor.
		/// </value>
		public SensorTypes SensorType { get; set; }

		/// <summary>
		/// Gets or sets the mac address.
		/// </summary>
		/// <value>
		/// The mac address.
		/// </value>
		public long? MacAddress { get; set; }
		/// <summary>
		/// Gets or sets the UUID.
		/// </summary>
		/// <value>
		/// The UUID.
		/// </value>
		public Guid? Uuid { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public double Value { get; set; }

		/// <summary>
		/// Gets or sets the value2.
		/// </summary>
		/// <value>
		/// The value2.
		/// </value>
		public double? Value2 { get; set; }
		/// <summary>
		/// Gets or sets the value3.
		/// </summary>
		/// <value>
		/// The value3.
		/// </value>
		public double? Value3 { get; set; }
		/// <summary>
		/// Gets or sets the value4.
		/// </summary>
		/// <value>
		/// The value4.
		/// </value>
		public double? Value4 { get; set; }
		/// <summary>
		/// Gets or sets the value5.
		/// </summary>
		/// <value>
		/// The value5.
		/// </value>
		public double? Value5 { get; set; }
		/// <summary>
		/// Gets or sets the value6.
		/// </summary>
		/// <value>
		/// The value6.
		/// </value>
		public double? Value6 { get; set; }
		/// <summary>
		/// Gets or sets the value7.
		/// </summary>
		/// <value>
		/// The value7.
		/// </value>
		public double? Value7 { get; set; }
		/// <summary>
		/// Gets or sets the value8.
		/// </summary>
		/// <value>
		/// The value8.
		/// </value>
		public double? Value8 { get; set; }
		/// <summary>
		/// Gets or sets the value9.
		/// </summary>
		/// <value>
		/// The value9.
		/// </value>
		public double? Value9 { get; set; }
		/// <summary>
		/// Gets or sets the value10.
		/// </summary>
		/// <value>
		/// The value10.
		/// </value>
		public double? Value10 { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>
		/// The created date.
		/// </value>
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return $"LocalId {LocalId}, SensorType {SensorType}, DeviceId DeviceId, Value = {Value}, CreatedDate = {CreatedDate}";
		}
	}
}
