using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class RulesetParametersAndModelFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches");

            migrationBuilder.AddColumn<bool>(
                name: "ChangedRemotely",
                table: "Matches",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RulesetParametersJSON",
                table: "Competitions",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ChangedRemotely",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RulesetParametersJSON",
                table: "Competitions");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id");
        }
    }
}
