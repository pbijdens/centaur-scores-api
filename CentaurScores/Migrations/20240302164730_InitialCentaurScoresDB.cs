using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class InitialCentaurScoresDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MatchCode = table.Column<string>(type: "longtext", nullable: false),
                    MatchName = table.Column<string>(type: "longtext", nullable: false),
                    NumberOfEnds = table.Column<int>(type: "int", nullable: false),
                    ArrowsPerEnd = table.Column<int>(type: "int", nullable: false),
                    AutoProgressAfterEachArrow = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ScoreValuesJson = table.Column<string>(type: "longtext", nullable: false),
                    GroupsJSON = table.Column<string>(type: "longtext", nullable: false),
                    SubgroupsJSON = table.Column<string>(type: "longtext", nullable: false),
                    LijnenJSON = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    JsonValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Name);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    DeviceID = table.Column<string>(type: "longtext", nullable: false),
                    Lijn = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Group = table.Column<string>(type: "longtext", nullable: false),
                    Subgroup = table.Column<string>(type: "longtext", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    EndsJSON = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_MatchId",
                table: "Participants",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
