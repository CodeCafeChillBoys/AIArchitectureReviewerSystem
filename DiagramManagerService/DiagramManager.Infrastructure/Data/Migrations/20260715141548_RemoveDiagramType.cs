using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiagramManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDiagramType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagramType",
                table: "DIAGRAMS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiagramType",
                table: "DIAGRAMS",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
