using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentaurScores.Migrations
{
    /// <inheritdoc />
    public partial class InactiveParticipantsLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInactive",
                table: "ParticipantLists",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInactive",
                table: "ParticipantLists");
        }
    }
}
