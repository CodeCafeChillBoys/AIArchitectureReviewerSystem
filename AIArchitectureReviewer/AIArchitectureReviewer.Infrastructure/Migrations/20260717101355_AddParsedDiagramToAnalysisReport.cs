using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIArchitectureReviewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParsedDiagramToAnalysisReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParsedDiagram",
                table: "ANALYSIS_REPORTS",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParsedDiagram",
                table: "ANALYSIS_REPORTS");
        }
    }
}
