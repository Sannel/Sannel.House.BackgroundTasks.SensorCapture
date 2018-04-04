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
					LocalId = table.Column<Guid>(nullable: false),
					CreatedDate = table.Column<DateTime>(nullable: false),
					MacAddress = table.Column<long>(nullable: true),
					SensorType = table.Column<int>(nullable: false),
					Uuid = table.Column<Guid>(nullable: true),
					Value = table.Column<double>(nullable: false),
					Value10 = table.Column<double>(nullable: true),
					Value2 = table.Column<double>(nullable: true),
					Value3 = table.Column<double>(nullable: true),
					Value4 = table.Column<double>(nullable: true),
					Value5 = table.Column<double>(nullable: true),
					Value6 = table.Column<double>(nullable: true),
					Value7 = table.Column<double>(nullable: true),
					Value8 = table.Column<double>(nullable: true),
					Value9 = table.Column<double>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SensorEntries", x => x.LocalId);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "SensorEntries");
		}
	}
}
