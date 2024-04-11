using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vocab.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGuessingLevelColumnAndConstraintCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuessingLevel",
                table: "StatementPairs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "GuessingLevelCheck",
                table: "StatementPairs",
                sql: "\"GuessingLevel\" >= 1 AND \"GuessingLevel\" <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "GuessingLevelCheck",
                table: "StatementPairs");

            migrationBuilder.DropColumn(
                name: "GuessingLevel",
                table: "StatementPairs");
        }
    }
}
