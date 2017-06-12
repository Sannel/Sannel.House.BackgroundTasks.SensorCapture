using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sannel.House.SensorCapture.Data.Models;

namespace Sannel.House.SensorCapture.Data
{
	public sealed class DataContext : DbContext
	{
		public DbSet<SensorEntry> SensorEntries { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=Entries.db");
		}
	}
}
