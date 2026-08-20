using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiagramManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDiagramsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WORKSPACES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKSPACES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DIAGRAMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DiagramType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIAGRAMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DIAGRAMS_WORKSPACES_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "WORKSPACES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DIAGRAM_SHARES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramId = table.Column<Guid>(type: "uuid", nullable: false),
                    SharedWithUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIAGRAM_SHARES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DIAGRAM_SHARES_DIAGRAMS_DiagramId",
                        column: x => x.DiagramId,
                        principalTable: "DIAGRAMS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DIAGRAM_VERSIONS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagramId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    StorageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RawFormat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIAGRAM_VERSIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DIAGRAM_VERSIONS_DIAGRAMS_DiagramId",
                        column: x => x.DiagramId,
                        principalTable: "DIAGRAMS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DIAGRAM_SHARES_DiagramId",
                table: "DIAGRAM_SHARES",
                column: "DiagramId");

            migrationBuilder.CreateIndex(
                name: "IX_DIAGRAM_VERSIONS_DiagramId",
                table: "DIAGRAM_VERSIONS",
                column: "DiagramId");

            migrationBuilder.CreateIndex(
                name: "IX_DIAGRAMS_WorkspaceId",
                table: "DIAGRAMS",
                column: "WorkspaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DIAGRAM_SHARES");

            migrationBuilder.DropTable(
                name: "DIAGRAM_VERSIONS");

            migrationBuilder.DropTable(
                name: "DIAGRAMS");

            migrationBuilder.DropTable(
                name: "WORKSPACES");
        }
    }
}
