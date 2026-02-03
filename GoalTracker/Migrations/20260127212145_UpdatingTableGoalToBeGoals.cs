using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingTableGoalToBeGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoalTasks_Goal_GoalId",
                table: "GoalTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Goal",
                table: "Goal");

            migrationBuilder.RenameTable(
                name: "Goal",
                newName: "Goals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoalTasks_Goals_GoalId",
                table: "GoalTasks",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoalTasks_Goals_GoalId",
                table: "GoalTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.RenameTable(
                name: "Goals",
                newName: "Goal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goal",
                table: "Goal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoalTasks_Goal_GoalId",
                table: "GoalTasks",
                column: "GoalId",
                principalTable: "Goal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
