using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
				optionsBuilder.UseSqlite("Data Source=test.db");
			}
		}
	}
}
