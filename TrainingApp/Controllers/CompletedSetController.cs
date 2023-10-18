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

        [HttpGet("{Id}", Name = "GetCompletedSetById")]
        [ProducesResponseType(typeof(CompletedSet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompletedSet([FromRoute] int Id)
        {
            CompletedSet? completedSet = await _dataBase.CompletedSets.FindAsync(Id);
            return completedSet == null ? NotFound() : Ok(completedSet);
        }

        [HttpGet("{SessionId}", Name = "GetCompletedSetsBySessionId")]
        [ProducesResponseType(typeof(IEnumerable<CompletedSet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int SessionId)
        {
            return Ok(_dataBase.CompletedSets.Where(set => set.WorkoutSessionId == SessionId).ToListAsync());
        }


        [HttpPost(Name = "CreateSet")]
        [ProducesResponseType(StatusCodes.Status201Created)]
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
            await _dataBase.CompletedSets.AddAsync(set);
            await _dataBase.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompletedSet), new { Id = set.SetId }, set);
        }
    }


}
