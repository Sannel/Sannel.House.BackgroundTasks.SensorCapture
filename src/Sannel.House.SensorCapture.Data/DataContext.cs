/* Copyright 2018 Sannel Software, L.L.C.

	Licensed under the Apache License, Version 2.0 (the ""License"");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an ""AS IS"" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.*/
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
