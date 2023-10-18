using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class functionNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentExerciseId",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentExerciseId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sessions");
        }
    }
}
