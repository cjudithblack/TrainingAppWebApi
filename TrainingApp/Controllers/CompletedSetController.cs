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
        [ProducesResponseType(typeof(IEnumerable<GroupedSetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int Id)
        {
            var groupedSets = await _dataBase.CompletedSets
                .Where(set => set.WorkoutSessionId == Id)
                .GroupBy(set => set.ExerciseId)
                .Select(group => new GroupedSetResponse
                {
                    Exercise = _dataBase.Exercises.FirstOrDefault(exercise => exercise.ExerciseId == group.Key),
                    Sets = group.ToList()
                })
                .ToListAsync();

            return Ok(groupedSets);
        }
        
        [HttpPut("Update/{SessionId}/{ExerciseId}", Name = "UpdateAllSetsInSession")]
        public async Task<IActionResult> UpdateAllExercisesInWorkout([FromRoute] int SessionId, [FromRoute] int ExerciseId, [FromBody] List<CompletedSetAdd> newCompletedSets)
        {
            var currentSetList = await _dataBase.CompletedSets.Where(set => set.WorkoutSessionId == SessionId && set.ExerciseId == ExerciseId).ToListAsync();
            foreach (var set in currentSetList)
            {
                _dataBase.Remove(set);
            }
            foreach (var set in newCompletedSets)
            {
                CompletedSet completedSet = new CompletedSet
                {
                    ExerciseId = ExerciseId,
                    WorkoutSessionId = SessionId,
                    Reps = set.Reps,
                    Weight = set.Weight,
                    Notes = set.Notes
                };
                Exercise? exercise = await _dataBase.Exercises.FindAsync(ExerciseId);
                Session? session = await _dataBase.Sessions.FindAsync(SessionId);
                if (session == null || exercise == null)
                    return BadRequest(ModelState);
                await _dataBase.CompletedSets.AddAsync(completedSet);
                await _dataBase.SaveChangesAsync();
            }
            return Ok();
        }


    }


}
