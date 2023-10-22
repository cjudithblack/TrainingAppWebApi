using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ExerciseInWorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        public ExerciseInWorkoutController(ApplicationDbContext db) => _dataBase = db;

        [HttpGet("Workout/{workoutId}", Name = "GetExerciseInWorkoutByWorkoutId")]
        [ProducesResponseType(typeof(IEnumerable<ExerciseInWorkout>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] int workoutId)
        {
            return Ok(_dataBase.ExerciseInWorkouts.Where(exercise => exercise.WorkoutId == workoutId).ToList());
        }

        [HttpGet("{WorkoutId}/{ExerciseId}", Name = "GetExerciseInWorkout")]
        [ProducesResponseType(typeof(ExerciseInWorkout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExerciseInWorkout([FromRoute] int WorkoutId, [FromRoute] int ExerciseId)
        {
            ExerciseInWorkout? exerciseInWorkout = await _dataBase.ExerciseInWorkouts.FirstOrDefaultAsync(e => e.WorkoutId == WorkoutId && e.ExerciseId == ExerciseId);
            if (exerciseInWorkout == null)
            {
                return NotFound();
            }
            return Ok(exerciseInWorkout);
        }

        [HttpGet("Exercise/{exerciseId}", Name = "GetRelatedWorkouts")]
        [ProducesResponseType(typeof(IEnumerable<ExerciseInWorkout>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRelatedWorkouts([FromRoute] int exerciseId)
        {
            List<ExerciseInWorkout> exerciseInWorkouts = await _dataBase.ExerciseInWorkouts.Where(e => e.ExerciseId == exerciseId).ToListAsync();
            if (exerciseInWorkouts.Count == 0)
                return NotFound();
            return Ok(exerciseInWorkouts);
        }

        [HttpPost("{WorkoutId}/{ExerciseId}", Name = "CreateExerciseInWorkout")]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<IActionResult> Create([FromRoute] int WorkoutId, [FromRoute] int ExerciseId, [FromBody] ExerciseInWorkoutAdd newExerciseInWorkout)
        {
            ExerciseInWorkout exerciseInWorkout = new ExerciseInWorkout { 
                WorkoutId = WorkoutId, 
                ExerciseId = ExerciseId,
                NumOfReps = newExerciseInWorkout.NumOfReps,
                NumOfSets = newExerciseInWorkout.NumOfSets,
                RestTime = newExerciseInWorkout.RestTime
            };

            Workout? workout = await _dataBase.Workouts.FindAsync(WorkoutId);
            Exercise? exercise = await _dataBase.Exercises.FindAsync(ExerciseId);
            if (workout == null || exercise == null)
                return BadRequest(ModelState);
            await _dataBase.ExerciseInWorkouts.AddAsync(exerciseInWorkout);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExerciseInWorkout), new { WorkoutId = WorkoutId, ExerciseId = ExerciseId }, exerciseInWorkout);
        }

        [HttpPut("Update/{WorkoutId}/{ExerciseId}", Name = "UpdateExerciseInWorkout")]
        public async Task<IActionResult> UpdateExerciseInWorkout([FromRoute] int WorkoutId, [FromRoute] int ExerciseId, [FromBody] ExerciseInWorkoutUpdate updatedExerciseInWorkout)
        {
            ExerciseInWorkout? exerciseInWorkout = await _dataBase.ExerciseInWorkouts.FirstOrDefaultAsync(e => e.WorkoutId == WorkoutId && e.ExerciseId == ExerciseId);
            if (exerciseInWorkout != null)
            {
                exerciseInWorkout.NumOfReps = updatedExerciseInWorkout.NumOfReps;
                exerciseInWorkout.NumOfSets = updatedExerciseInWorkout.NumOfSets;
                exerciseInWorkout.RestTime = updatedExerciseInWorkout.RestTime;
                await _dataBase.SaveChangesAsync();
                return Ok(exerciseInWorkout);
            }
            else
                return NotFound();
        }

        [HttpDelete("Delete/{WorkoutId}/{ExerciseId}", Name = "DeleteExerciseInWorkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePlan([FromRoute] int WorkoutId, [FromRoute] int ExerciseId)
        {
            ExerciseInWorkout? exerciseInWorkout = await _dataBase.ExerciseInWorkouts.FirstOrDefaultAsync(e => e.WorkoutId == WorkoutId && e.ExerciseId == ExerciseId);
            if (exerciseInWorkout != null)
            {
                _dataBase.Remove(exerciseInWorkout);
                _dataBase.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
