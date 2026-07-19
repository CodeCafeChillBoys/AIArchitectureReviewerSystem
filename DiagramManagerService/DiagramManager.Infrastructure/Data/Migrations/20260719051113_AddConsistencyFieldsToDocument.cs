using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiagramManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsistencyFieldsToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConsistencyReview",
                table: "DOCUMENTS",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ConsistencyScore",
                table: "DOCUMENTS",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsistencyReview",
                table: "DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "ConsistencyScore",
                table: "DOCUMENTS");
        }
    }
}
