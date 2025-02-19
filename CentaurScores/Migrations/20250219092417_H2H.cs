using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class H2H : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeadToHeadJSON",
                table: "Participants",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "ActiveRound",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "MatchFlags",
                table: "Matches",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRounds",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadToHeadJSON",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ActiveRound",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchFlags",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "NumberOfRounds",
                table: "Matches");
        }
    }
}
