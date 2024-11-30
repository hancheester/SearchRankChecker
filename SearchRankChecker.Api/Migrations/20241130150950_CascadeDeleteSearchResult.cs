using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchRankChecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteSearchResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchResultEntry_SearchHistory",
                table: "SearchResultEntry");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchResultEntry_SearchHistory",
                table: "SearchResultEntry",
                column: "SearchHistoryId",
                principalTable: "SearchHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchResultEntry_SearchHistory",
                table: "SearchResultEntry");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchResultEntry_SearchHistory",
                table: "SearchResultEntry",
                column: "SearchHistoryId",
                principalTable: "SearchHistory",
                principalColumn: "Id");
        }
    }
}
