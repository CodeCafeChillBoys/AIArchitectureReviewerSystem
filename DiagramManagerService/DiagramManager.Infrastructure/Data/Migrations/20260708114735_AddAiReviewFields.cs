using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiagramManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAiReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiReview",
                table: "DIAGRAM_VERSIONS",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AiScore",
                table: "DIAGRAM_VERSIONS",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiReview",
                table: "DIAGRAM_VERSIONS");

            migrationBuilder.DropColumn(
                name: "AiScore",
                table: "DIAGRAM_VERSIONS");
        }
    }
}
