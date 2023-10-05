using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Data;
using TrainingApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Numerics;
using TrainingApp.Data.Migrations;

namespace TrainingApp.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;

        public WorkoutController(ApplicationDbContext db) => _dataBase = db;


        [HttpGet("/Plans/{planId}/Workouts")]
        [Authorize]
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
            return Ok(await _dataBase.Workouts.Where(workout => workout.PlanId == planId).ToListAsync());
        }


        [HttpGet("/Plans/{planId}/Workouts/{id}")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWorkout([FromRoute] int planId, [FromRoute] int id)
        {
            var workout = await _dataBase.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            if (workout.PlanId != planId)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan?.UserId != userId)
            {
                return BadRequest();
            }
            return Ok(workout);
        }

        [HttpPost("/Plans/{planId}/CreateWorkout")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(AddWorkout newWorkout, [FromRoute] int planId)
        {
            Workout workout = new Workout {
                Name = newWorkout.Name,
                Description = newWorkout.Description,
                PlanId = planId };
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan == null)
                return BadRequest(ModelState);
            List<Workout> PlansWorkouts = await _dataBase.Workouts.Where(workout => workout.PlanId == plan.PlanId).ToListAsync();
            if (PlansWorkouts?.Count == 0) //when creating the first workout - it will be the current workout
                plan.CurrentWorkoutId = workout.WorkoutId;
            await _dataBase.Workouts.AddAsync(workout);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkout), new { id = workout.WorkoutId }, workout);
        }

        [HttpPatch("StartWorkout/{id}")]
        public async Task<IActionResult> StartWorkout([FromRoute] int id)
        {
            Workout? workout = await _dataBase.Workouts.FindAsync(id);
            if (workout == null)
                return NotFound();
            Plan currentPlan = await _dataBase.Plans.FirstAsync(plan => workout.PlanId == plan.PlanId);
            if (currentPlan == null)
                return BadRequest(ModelState);
            currentPlan.CurrentWorkoutId = id;
            return Ok(workout);
        }

        [HttpPut("UpdateWorkout/{id}")]
        public async Task<IActionResult> updateWorkout([FromRoute] int id, [FromBody] UpdateWorkout updatedWorkout)
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

        [HttpDelete("DeleteWorkout/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeletePlan([FromRoute] int id)
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


