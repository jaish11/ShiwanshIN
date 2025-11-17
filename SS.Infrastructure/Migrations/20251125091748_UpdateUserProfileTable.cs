using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GPA",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "GPA",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "University",
                table: "UserProfiles");
        }
    }
}
