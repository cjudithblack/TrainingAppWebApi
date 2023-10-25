using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class lastweight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedSets_Exercises_ExerciseId",
                table: "CompletedSets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Workouts_WorkoutId",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "LastWeight",
                table: "Exercises",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedSets_Exercises_ExerciseId",
                table: "CompletedSets",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Workouts_WorkoutId",
                table: "Sessions",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "WorkoutId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedSets_Exercises_ExerciseId",
                table: "CompletedSets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Workouts_WorkoutId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LastWeight",
                table: "Exercises");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedSets_Exercises_ExerciseId",
                table: "CompletedSets",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Workouts_WorkoutId",
                table: "Sessions",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "WorkoutId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
