using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchRankChecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchEngineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchEngine",
                table: "SearchHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchEngine",
                table: "SearchHistory");
        }
    }
}
