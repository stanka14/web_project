using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Projekat_BackEnd.Migrations
{
    public partial class RowVersionCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RentACarCompanies",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RentACarCompanies");
        }
    }
}
