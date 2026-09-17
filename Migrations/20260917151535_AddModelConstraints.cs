using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRankingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddModelConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_GameName",
                table: "PlayerScores",
                column: "GameName");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_PlayerName",
                table: "PlayerScores",
                column: "PlayerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerScores_GameName",
                table: "PlayerScores");

            migrationBuilder.DropIndex(
                name: "IX_PlayerScores_PlayerName",
                table: "PlayerScores");
        }
    }
}
