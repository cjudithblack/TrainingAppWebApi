using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;

        public WorkoutController(ApplicationDbContext db) => _dataBase = db;


        [HttpGet]
        public async Task<IEnumerable<Workout>> Get()
        {
            return await _dataBase.Workouts.ToListAsync();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getWorkout(int id)
        {
            var workout = await _dataBase.Workouts.FindAsync(id);
            return workout == null ? NotFound() : Ok(workout);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Workout workout)
        {
            await _dataBase.Workouts.AddAsync(workout);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(getWorkout), new { id = workout.WorkoutId }, workout);
        }
    }
}


/* public async Task<Workout> Create(Workout workout)
 {
     _dataBase.Workouts.Add(workout);
     await _dataBase.SaveChangesAsync();
     return workout;
 }
 /*
 [HttpPatch]
 [Route("UpdateWorkout/{id}")]
 public async Task<Workout> updateWorkout(Workout workout)
 {
     _dataBase.Entry(workout).State = EntityState.Modified;
     await _dataBase.SaveChangesAsync();
     return workout;
 }

 [HttpDelete]
 [Route("DeleteWorkout/{id}")]
 public bool deleteWorkout(int id)
 {
     var workout = _dataBase.Workouts.Find(id);
     if (workout != null)
     {
         _dataBase.Entry(workout).State = EntityState.Deleted;
         _dataBase.SaveChanges();
         return true;
     }
     return false;
 }*/