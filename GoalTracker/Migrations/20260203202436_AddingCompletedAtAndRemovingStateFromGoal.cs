using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddingCompletedAtAndRemovingStateFromGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Goals");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Goals",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Goals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
