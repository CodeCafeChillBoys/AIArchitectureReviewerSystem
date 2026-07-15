using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIArchitectureReviewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AI_PROMPTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramType = table.Column<string>(type: "text", nullable: false),
                    PromptText = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_PROMPTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_RULES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramType = table.Column<string>(type: "text", nullable: false),
                    RuleName = table.Column<string>(type: "text", nullable: false),
                    RegexOrCondition = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_RULES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ANALYSIS_REPORTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AiPromptId = table.Column<Guid>(type: "uuid", nullable: false),
                    RawAiResponse = table.Column<string>(type: "text", nullable: false),
                    MarkdownReport = table.Column<string>(type: "text", nullable: false),
                    TotalScore = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANALYSIS_REPORTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ANALYSIS_REPORTS_AI_PROMPTS_AiPromptId",
                        column: x => x.AiPromptId,
                        principalTable: "AI_PROMPTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FEEDBACKS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FEEDBACKS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FEEDBACKS_ANALYSIS_REPORTS_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ANALYSIS_REPORTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "REPORT_DETAILS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ElementId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_DETAILS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_REPORT_DETAILS_ANALYSIS_REPORTS_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ANALYSIS_REPORTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ANALYSIS_REPORTS_AiPromptId",
                table: "ANALYSIS_REPORTS",
                column: "AiPromptId");

            migrationBuilder.CreateIndex(
                name: "IX_FEEDBACKS_ReportId",
                table: "FEEDBACKS",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_REPORT_DETAILS_ReportId",
                table: "REPORT_DETAILS",
                column: "ReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FEEDBACKS");

            migrationBuilder.DropTable(
                name: "REPORT_DETAILS");

            migrationBuilder.DropTable(
                name: "SYSTEM_RULES");

            migrationBuilder.DropTable(
                name: "ANALYSIS_REPORTS");

            migrationBuilder.DropTable(
                name: "AI_PROMPTS");
        }
    }
}
