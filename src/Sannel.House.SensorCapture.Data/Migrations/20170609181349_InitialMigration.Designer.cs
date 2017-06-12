using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Sannel.House.SensorCapture.Data;
using Sannel.House.SensorCapture.Data.Models;

namespace Sannel.House.SensorCapture.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20170609181349_InitialMigration")]
    partial class InitialMigration
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

                    b.HasKey("LocalId");

                    b.ToTable("SensorEntries");
                });
        }
    }
}
