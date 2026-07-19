using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiagramManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "DIAGRAMS",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DOCUMENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StorageUrl = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCUMENTS_WORKSPACES_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "WORKSPACES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DIAGRAMS_DocumentId",
                table: "DIAGRAMS",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENTS_WorkspaceId",
                table: "DOCUMENTS",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DIAGRAMS_DOCUMENTS_DocumentId",
                table: "DIAGRAMS",
                column: "DocumentId",
                principalTable: "DOCUMENTS",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DIAGRAMS_DOCUMENTS_DocumentId",
                table: "DIAGRAMS");

            migrationBuilder.DropTable(
                name: "DOCUMENTS");

            migrationBuilder.DropIndex(
                name: "IX_DIAGRAMS_DocumentId",
                table: "DIAGRAMS");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DIAGRAMS");
        }
    }
}
