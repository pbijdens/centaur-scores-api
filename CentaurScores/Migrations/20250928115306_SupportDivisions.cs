using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class SupportDivisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJSON",
                table: "ParticipantLists",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompetitionFormatDisciplineDivisionMapJSON",
                table: "ParticipantListEntries",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationJSON",
                table: "ParticipantLists");

            migrationBuilder.DropColumn(
                name: "CompetitionFormatDisciplineDivisionMapJSON",
                table: "ParticipantListEntries");
        }
    }
}
