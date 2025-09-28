using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class InactiveCompetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInactive",
                table: "Competitions",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInactive",
                table: "Competitions");
        }
    }
}
