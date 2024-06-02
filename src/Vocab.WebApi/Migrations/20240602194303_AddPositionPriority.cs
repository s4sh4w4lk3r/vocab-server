using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vocab.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionPriority",
                table: "StatementPairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionPriority",
                table: "StatementDictionaries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionPriority",
                table: "StatementPairs");

            migrationBuilder.DropColumn(
                name: "PositionPriority",
                table: "StatementDictionaries");
        }
    }
}
