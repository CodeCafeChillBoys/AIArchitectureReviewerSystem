using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIArchitectureReviewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROMPT_TEMPLATE_HISTORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromptTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UnifiedDiff = table.Column<string>(type: "text", nullable: true),
                    Additions = table.Column<int>(type: "integer", nullable: false),
                    Deletions = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROMPT_TEMPLATE_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PROMPT_TEMPLATE_HISTORY_PROMPT_TEMPLATES_PromptTemplateId",
                        column: x => x.PromptTemplateId,
                        principalTable: "PROMPT_TEMPLATES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_RULE_HISTORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UnifiedDiff = table.Column<string>(type: "text", nullable: true),
                    Additions = table.Column<int>(type: "integer", nullable: false),
                    Deletions = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_RULE_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SYSTEM_RULE_HISTORY_SYSTEM_RULES_SystemRuleId",
                        column: x => x.SystemRuleId,
                        principalTable: "SYSTEM_RULES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PROMPT_TEMPLATE_HISTORY_PromptTemplateId_ChangedAt",
                table: "PROMPT_TEMPLATE_HISTORY",
                columns: new[] { "PromptTemplateId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_RULE_HISTORY_SystemRuleId_ChangedAt",
                table: "SYSTEM_RULE_HISTORY",
                columns: new[] { "SystemRuleId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROMPT_TEMPLATE_HISTORY");

            migrationBuilder.DropTable(
                name: "SYSTEM_RULE_HISTORY");
        }
    }
}
