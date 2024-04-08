using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vocab.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatementDictionaries",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementDictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatementPairs",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    Target = table.Column<string>(type: "text", nullable: false),
                    StatementCategory = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RelatedDictionaryId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementPairs_StatementDictionaries_RelatedDictionaryId",
                        column: x => x.RelatedDictionaryId,
                        principalTable: "StatementDictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatementPairs_RelatedDictionaryId",
                table: "StatementPairs",
                column: "RelatedDictionaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatementPairs");

            migrationBuilder.DropTable(
                name: "StatementDictionaries");
        }
    }
}
