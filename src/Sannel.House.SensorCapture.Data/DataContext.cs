using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sannel.House.Sensor;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

		public DbSet<SensorEntry> SensorEntries { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlite("Data Source=Data.db");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SensorEntry>(i =>
			{
				i.HasKey(j => j.Id);
				i.Ignore(j => j.ExtraElements);
				i.Ignore(j => j.DeviceId);

				var dictionaryToString = new ValueConverter<IDictionary<string, float>, string>(
								v => v.ConvertToString(),
								v => JsonConvert.DeserializeObject<Dictionary<string,float>>(v));

				i.Property(j => j.Values)
				.HasConversion(dictionaryToString);
			});

		}
	}
}
