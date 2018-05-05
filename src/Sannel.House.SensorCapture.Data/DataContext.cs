using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sannel.House.Sensor;
using Newtonsoft.Json;

namespace Sannel.House.SensorCapture.Data
{
	public sealed class DataContext : DbContext
	{
		public DataContext()
		{
		}
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}

		public DbSet<LocalSensorEntry> SensorEntries { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlite("Data Source=Data.db");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<LocalSensorEntry>(i =>
			{
				i.HasKey(j => j.Id);
				i.Ignore(j => j.ExtraElements);
				i.Ignore(j => j.DeviceId);
				i.Ignore(j => j.Values);
			});

		}
	}
}
