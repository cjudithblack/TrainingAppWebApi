using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Data;
using TrainingApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Numerics;
using TrainingApp.Data.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TrainingApp.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class WorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;

        public WorkoutController(ApplicationDbContext db) => _dataBase = db;


        [HttpGet("/Plans/{planId}/Workouts", Name = "GetWorkoutsByPlanId")]
        [ProducesResponseType(typeof(IEnumerable<Workout>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] int planId)
        {
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan == null)
                return NotFound();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || plan.UserId != userId)
            {
                return BadRequest("Invalid User");
            }
            return Ok(await _dataBase.Workouts
                .Include(w => w.Plan)
                .Where(workout => workout.PlanId == planId)
                .OrderBy(workout => workout.WorkoutId)
                .ToListAsync());
        }


        [HttpGet("Workouts/{id}", Name = "GetWorkoutbyId")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWorkout([FromRoute] int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = await _dataBase.Users.FindAsync(userId);
            var workout = await _dataBase.Workouts
                .Include(w => w.Plan)
                .FirstOrDefaultAsync(w => w.WorkoutId == id);
            if (workout == null)
            {
                return NotFound();
            }
            Plan? plan = await _dataBase.Plans.FirstOrDefaultAsync(p => p.PlanId == workout.PlanId);
            if (plan?.UserId != userId)
            {
                return BadRequest();
            }
            return Ok(workout);
        }

        [HttpGet("Plans/{planId}/NextWorkout", Name = "GetNextWorkout")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNextWorkout([FromRoute] int planId)
        {
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan == null)
                return NotFound();
            if (plan?.NextWorkoutId == null)
                return NotFound("No workouts");
            Workout? workout = await _dataBase.Workouts
                .Include(w => w.Plan)
                .FirstOrDefaultAsync(w => w.WorkoutId == plan.NextWorkoutId);
            return workout == null ? NotFound() : Ok(workout);
        }

        [HttpPost("/Plans/{planId}/CreateWorkout", Name = "CreateWorkout")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(WorkoutAdd newWorkout, [FromRoute] int planId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? plansUser = _dataBase.Plans.Find(planId)?.UserId;
            if (plansUser != userId)
                return NotFound();
            Workout? workoutNameExists = _dataBase.Workouts
                .FirstOrDefaultAsync(workout => workout.PlanId == planId && workout.Name == newWorkout.Name).Result;
            if (workoutNameExists != null)
                return BadRequest("You already have a workout with the same name");
            Workout workout = new Workout
            {
                Name = newWorkout.Name,
                Description = newWorkout.Description,
                PlanId = planId
            };
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan == null)
                return BadRequest(ModelState);
            await _dataBase.Workouts.AddAsync(workout);
            await _dataBase.SaveChangesAsync();
            if (plan.Workouts.Count == 1) //when creating the first workout - it will be the next workout
                plan.NextWorkoutId = workout.WorkoutId;
            await _dataBase.SaveChangesAsync();
            return CreatedAtAction("GetWorkout", new { id = workout.WorkoutId }, workout);

        }


        [HttpPut("UpdateWorkout/{id}", Name = "UpdateWorkoutById")]
        public async Task<IActionResult> UpdateWorkout([FromRoute] int id, [FromBody] WorkoutUpdate updatedWorkout)
        {
            var workout = await _dataBase.Workouts.FindAsync(id);
            if (workout != null)
            {
                workout.Name = updatedWorkout.Name;
                workout.Description = updatedWorkout.Description;
                await _dataBase.SaveChangesAsync();
                return Ok(workout);
            }
            else
                return NotFound();
        }

        [HttpDelete("DeleteWorkout/{id}", Name = "DeleteWorkoutById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var workout = await _dataBase.Workouts.FindAsync(id);
            if (workout != null)
            {
                _dataBase.Remove(workout);
                _dataBase.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

    }
}


