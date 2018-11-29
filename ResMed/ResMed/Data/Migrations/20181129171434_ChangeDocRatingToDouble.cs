using Microsoft.EntityFrameworkCore.Migrations;

namespace ResMed.Data.Migrations
{
    public partial class ChangeDocRatingToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AverageRating",
                table: "Doctors",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AverageRating",
                table: "Doctors",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
