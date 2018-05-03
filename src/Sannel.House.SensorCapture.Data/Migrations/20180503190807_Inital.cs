using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sannel.House.SensorCapture.Data.Migrations
{
	public partial class Inital : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "SensorEntries",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					DateCreated = table.Column<DateTime>(nullable: false),
					DeviceMacAddress = table.Column<long>(nullable: true),
					DeviceUuid = table.Column<Guid>(nullable: true),
					SValues = table.Column<string>(nullable: true),
					SensorType = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SensorEntries", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "SensorEntries");
		}
	}
}
