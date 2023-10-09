using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class finalChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedSets_Sessions_ParentWorkoutSessionDate_ParentWorkoutSessionWorkoutId",
                table: "CompletedSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_CompletedSets_ParentWorkoutSessionDate_ParentWorkoutSessionWorkoutId",
                table: "CompletedSets");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "CompletedSets");

            migrationBuilder.DropColumn(
                name: "ParentWorkoutSessionDate",
                table: "CompletedSets");

            migrationBuilder.DropColumn(
                name: "ParentWorkoutSessionWorkoutId",
                table: "CompletedSets");

            migrationBuilder.RenameColumn(
                name: "WorkoutId",
                table: "CompletedSets",
                newName: "WorkoutSessionId");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "CompletedSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPlanId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedSets_WorkoutSessionId",
                table: "CompletedSets",
                column: "WorkoutSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedSets_Sessions_WorkoutSessionId",
                table: "CompletedSets",
                column: "WorkoutSessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedSets_Sessions_WorkoutSessionId",
                table: "CompletedSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_CompletedSets_WorkoutSessionId",
                table: "CompletedSets");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "CompletedSets");

            migrationBuilder.DropColumn(
                name: "CurrentPlanId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WorkoutSessionId",
                table: "CompletedSets",
                newName: "WorkoutId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CompletedSets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ParentWorkoutSessionDate",
                table: "CompletedSets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentWorkoutSessionWorkoutId",
                table: "CompletedSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                columns: new[] { "Date", "WorkoutId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedSets_ParentWorkoutSessionDate_ParentWorkoutSessionWorkoutId",
                table: "CompletedSets",
                columns: new[] { "ParentWorkoutSessionDate", "ParentWorkoutSessionWorkoutId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedSets_Sessions_ParentWorkoutSessionDate_ParentWorkoutSessionWorkoutId",
                table: "CompletedSets",
                columns: new[] { "ParentWorkoutSessionDate", "ParentWorkoutSessionWorkoutId" },
                principalTable: "Sessions",
                principalColumns: new[] { "Date", "WorkoutId" });
        }
    }
}
