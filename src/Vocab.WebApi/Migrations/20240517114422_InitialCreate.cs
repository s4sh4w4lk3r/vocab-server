﻿using System;
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatementCategory = table.Column<int>(type: "int", nullable: false),
                    GuessingLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedDictionaryId = table.Column<long>(type: "bigint", nullable: false)
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