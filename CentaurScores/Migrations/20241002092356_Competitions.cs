using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class Competitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompetitionId",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RulesetCode",
                table: "Matches",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    RulesetGroupName = table.Column<string>(type: "longtext", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    ParticipantListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Competitions_ParticipantLists_ParticipantListId",
                        column: x => x.ParticipantListId,
                        principalTable: "ParticipantLists",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CompetitionId",
                table: "Matches",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ParticipantListId",
                table: "Competitions",
                column: "ParticipantListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropIndex(
                name: "IX_Matches_CompetitionId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RulesetCode",
                table: "Matches");
        }
    }
}
