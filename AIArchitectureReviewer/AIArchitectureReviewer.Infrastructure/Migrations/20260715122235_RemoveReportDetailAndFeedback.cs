using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIArchitectureReviewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReportDetailAndFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FEEDBACKS");

            migrationBuilder.DropTable(
                name: "REPORT_DETAILS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FEEDBACKS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Description = table.Column<string>(type: "text", nullable: false),
                    ElementId = table.Column<string>(type: "text", nullable: false),
                    IssueType = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_FEEDBACKS_ReportId",
                table: "FEEDBACKS",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_REPORT_DETAILS_ReportId",
                table: "REPORT_DETAILS",
                column: "ReportId");
        }
    }
}
