using Microsoft.EntityFrameworkCore.Migrations;

namespace Projekat_BackEnd.Migrations
{
    public partial class korisnikSaSocijalneMreze : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SocialUser",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialUser",
                table: "AspNetUsers");
        }
    }
}
