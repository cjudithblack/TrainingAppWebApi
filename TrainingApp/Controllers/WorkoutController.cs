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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(WorkoutAdd newWorkout, [FromRoute] int planId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? plansUser = _dataBase.Plans.Find(planId)?.UserId;
            if (plansUser != userId)
                return NotFound();
            Workout? workoutNameExists = _dataBase.Workouts.FirstOrDefaultAsync(workout => workout.PlanId == planId && workout.Name == newWorkout.Name).Result;
            if (workoutNameExists != null)
                return BadRequest("You already have a workout with the same name");
            Workout workout = new Workout {
                Name = newWorkout.Name,
                Description = newWorkout.Description,
                PlanId = planId };
            Plan? plan = await _dataBase.Plans.FindAsync(planId);
            if (plan == null)
                return BadRequest(ModelState);
            await _dataBase.Workouts.AddAsync(workout);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkout), new { planId = planId, id = workout.WorkoutId }, workout);
        }


        [HttpPut("UpdateWorkout/{id}")]
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

        [HttpDelete("DeleteWorkout/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
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


