using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Projekat_BackEnd.Migrations
{
    public partial class migracija : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Flight",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AirlineCompanies",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AirlineCompanies");
        }
    }
}
