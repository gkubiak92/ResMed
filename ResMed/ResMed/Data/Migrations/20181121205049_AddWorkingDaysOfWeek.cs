using Microsoft.EntityFrameworkCore.Migrations;

namespace ResMed.Data.Migrations
{
    public partial class AddWorkingDaysOfWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WorkingFriday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingMonday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingSaturday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingThursday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingTuesday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkingWednesday",
                table: "Doctors",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingFriday",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkingMonday",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkingSaturday",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkingThursday",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkingTuesday",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkingWednesday",
                table: "Doctors");
        }
    }
}
