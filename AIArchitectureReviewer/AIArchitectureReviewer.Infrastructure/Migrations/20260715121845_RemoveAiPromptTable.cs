using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIArchitectureReviewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAiPromptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ANALYSIS_REPORTS_AI_PROMPTS_AiPromptId",
                table: "ANALYSIS_REPORTS");

            migrationBuilder.DropTable(
                name: "AI_PROMPTS");

            migrationBuilder.DropIndex(
                name: "IX_ANALYSIS_REPORTS_AiPromptId",
                table: "ANALYSIS_REPORTS");

            migrationBuilder.DropColumn(
                name: "AiPromptId",
                table: "ANALYSIS_REPORTS");

            migrationBuilder.AddColumn<string>(
                name: "DiagramType",
                table: "ANALYSIS_REPORTS",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagramType",
                table: "ANALYSIS_REPORTS");

            migrationBuilder.AddColumn<Guid>(
                name: "AiPromptId",
                table: "ANALYSIS_REPORTS",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AI_PROMPTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_PROMPTS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ANALYSIS_REPORTS_AiPromptId",
                table: "ANALYSIS_REPORTS",
                column: "AiPromptId");

            migrationBuilder.AddForeignKey(
                name: "FK_ANALYSIS_REPORTS_AI_PROMPTS_AiPromptId",
                table: "ANALYSIS_REPORTS",
                column: "AiPromptId",
                principalTable: "AI_PROMPTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
