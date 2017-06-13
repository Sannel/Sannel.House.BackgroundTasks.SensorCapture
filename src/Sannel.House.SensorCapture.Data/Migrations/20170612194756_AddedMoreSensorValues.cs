using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.SensorCapture.Data.Migrations
{
    public partial class AddedMoreSensorValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Value2",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value3",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value4",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value5",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value6",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value7",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value8",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Value9",
                table: "SensorEntries",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value2",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value3",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value4",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value5",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value6",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value7",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value8",
                table: "SensorEntries");

            migrationBuilder.DropColumn(
                name: "Value9",
                table: "SensorEntries");
        }
    }
}
