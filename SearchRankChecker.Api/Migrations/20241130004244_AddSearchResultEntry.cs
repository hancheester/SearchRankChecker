using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchRankChecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchResultEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ranks",
                table: "SearchHistory");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "SearchHistory",
                newName: "TargetUrl");

            migrationBuilder.CreateTable(
                name: "SearchResultEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchHistoryId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResultEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchResultEntry_SearchHistory",
                        column: x => x.SearchHistoryId,
                        principalTable: "SearchHistory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchResultEntry_SearchHistoryId",
                table: "SearchResultEntry",
                column: "SearchHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchResultEntry");

            migrationBuilder.RenameColumn(
                name: "TargetUrl",
                table: "SearchHistory",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "Ranks",
                table: "SearchHistory",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
