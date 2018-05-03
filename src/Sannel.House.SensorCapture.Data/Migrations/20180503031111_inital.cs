using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.SensorCapture.Data.Migrations
{
	public partial class inital : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "SensorEntries",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					SensorType = table.Column<string>(nullable: true),
					DeviceMacAddress = table.Column<long>(nullable: true),
					DeviceUuid = table.Column<Guid>(nullable: true),
					DateCreated = table.Column<DateTime>(nullable: false),
					Values = table.Column<string>(nullable: true)
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
