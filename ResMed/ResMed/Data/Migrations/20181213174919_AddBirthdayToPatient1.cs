using Microsoft.EntityFrameworkCore.Migrations;

namespace ResMed.Data.Migrations
{
    public partial class AddBirthdayToPatient1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthdayDate",
                table: "Patients",
                newName: "Birthday");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "Patients",
                newName: "BirthdayDate");
        }
    }
}
