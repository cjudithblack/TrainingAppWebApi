using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class dataBaseChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "CompletedSets");

            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "Exercises",
                newName: "VideoId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Exercises",
                newName: "Instructions");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentExerciseIndex",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "CompletedSets",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "Exercises",
                newName: "VideoUrl");

            migrationBuilder.RenameColumn(
                name: "Instructions",
                table: "Exercises",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentExerciseIndex",
                table: "Sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "CompletedSets",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "CompletedSets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
