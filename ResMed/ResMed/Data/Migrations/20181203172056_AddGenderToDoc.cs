using Microsoft.EntityFrameworkCore.Migrations;

namespace ResMed.Data.Migrations
{
    public partial class AddGenderToDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Doctors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Doctors");
        }
    }
}
