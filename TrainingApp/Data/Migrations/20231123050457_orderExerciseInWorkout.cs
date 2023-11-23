using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderExerciseInWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentExerciseId",
                table: "Sessions",
                newName: "CurrentExerciseIndex");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ExerciseInWorkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "ExerciseInWorkouts");

            migrationBuilder.RenameColumn(
                name: "CurrentExerciseIndex",
                table: "Sessions",
                newName: "CurrentExerciseId");
        }
    }
}
