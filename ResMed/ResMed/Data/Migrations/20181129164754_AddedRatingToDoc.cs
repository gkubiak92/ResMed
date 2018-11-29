using Microsoft.EntityFrameworkCore.Migrations;

namespace ResMed.Data.Migrations
{
    public partial class AddedRatingToDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageRating",
                table: "Doctors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Doctors",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Doctors");
        }
    }
}
