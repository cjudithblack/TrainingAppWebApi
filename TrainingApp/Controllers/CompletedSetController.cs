using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class CompletedSetController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        public CompletedSetController(ApplicationDbContext db) => _dataBase = db;

        [HttpGet("Set/{Id}", Name = "GetCompletedSetById")]
        [ProducesResponseType(typeof(CompletedSet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompletedSet([FromRoute] int Id)
        {
            CompletedSet? completedSet = await _dataBase.CompletedSets
                .Include(s => s.Exercise)
                .Include(s => s.ParentWorkoutSession)
                .FirstOrDefaultAsync(s => s.SetId == Id);
            return completedSet == null ? NotFound() : Ok(completedSet);
        }

        [HttpGet("Session/{Id}", Name = "GetCompletedSetsBySessionId")]
        [ProducesResponseType(typeof(IEnumerable<CompletedSet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            return Ok(
                await _dataBase.CompletedSets
                .Where(set => set.WorkoutSessionId == Id)
                .Include(s => s.Exercise)
                .Include(s => s.ParentWorkoutSession)
                .GroupBy(set => set.ExerciseId)
                .ToListAsync());
        }


        [HttpPost(Name = "CreateSet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CompletedSetAdd newCompletedSet)
        {
            CompletedSet set = new CompletedSet
            {
                WorkoutSessionId = newCompletedSet.WorkoutSessionId,
                ExerciseId = newCompletedSet.ExerciseId,
                Reps = newCompletedSet.Reps,
                Weight = newCompletedSet.Weight,
                Notes = newCompletedSet.Notes
            };

            Session? session = await _dataBase.Sessions.FindAsync(newCompletedSet.WorkoutSessionId);
            Exercise? exercise = await _dataBase.Exercises.FindAsync(newCompletedSet.ExerciseId);
            if (session == null || exercise == null)
                return BadRequest();
            exercise.LastWeight = newCompletedSet.Weight;
            await _dataBase.CompletedSets.AddAsync(set);
            await _dataBase.SaveChangesAsync();
            return Ok(set);
        }
    }


}
