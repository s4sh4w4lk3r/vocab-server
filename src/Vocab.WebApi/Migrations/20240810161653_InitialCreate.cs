using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Target = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    StatementCategory = table.Column<int>(type: "integer", nullable: false),
                    GuessingLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatementsDictionaryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementPairs_StatementDictionaries_StatementsDictionaryId",
                        column: x => x.StatementsDictionaryId,
                        principalTable: "StatementDictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatementPairs_StatementsDictionaryId",
                table: "StatementPairs",
                column: "StatementsDictionaryId");
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
