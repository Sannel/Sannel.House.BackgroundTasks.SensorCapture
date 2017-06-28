using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Sannel.House.SensorCapture.Data;
using Sannel.House.Sensor;

namespace Sannel.House.SensorCapture.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20170613193552_InitalMigration")]
    partial class InitalMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Sannel.House.SensorCapture.Data.Models.SensorEntry", b =>
                {
                    b.Property<Guid>("LocalId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DeviceId");

                    b.Property<int>("SensorType");

                    b.Property<double>("Value");

                    b.Property<double?>("Value2");

                    b.Property<double?>("Value3");

                    b.Property<double?>("Value4");

                    b.Property<double?>("Value5");

                    b.Property<double?>("Value6");

                    b.Property<double?>("Value7");

                    b.Property<double?>("Value8");

                    b.Property<double?>("Value9");

                    b.HasKey("LocalId");

                    b.ToTable("SensorEntries");
                });
        }
    }
}
