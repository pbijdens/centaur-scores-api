using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class PersonalBestScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "ParticipantListEntries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PersonalBestLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    CompetitionFormat = table.Column<string>(type: "longtext", nullable: false),
                    ParticipantListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalBestLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalBestLists_ParticipantLists_ParticipantListId",
                        column: x => x.ParticipantListId,
                        principalTable: "ParticipantLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PersonalBestListEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    Discipline = table.Column<string>(type: "longtext", nullable: false),
                    AchievedDate = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalBestListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalBestListEntries_ParticipantListEntries_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "ParticipantListEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonalBestListEntries_PersonalBestLists_ListId",
                        column: x => x.ListId,
                        principalTable: "PersonalBestLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalBestListEntries_ListId",
                table: "PersonalBestListEntries",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalBestListEntries_ParticipantId",
                table: "PersonalBestListEntries",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalBestLists_Name",
                table: "PersonalBestLists",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalBestLists_ParticipantListId",
                table: "PersonalBestLists",
                column: "ParticipantListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalBestListEntries");

            migrationBuilder.DropTable(
                name: "PersonalBestLists");

            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "ParticipantListEntries");
        }
    }
}
