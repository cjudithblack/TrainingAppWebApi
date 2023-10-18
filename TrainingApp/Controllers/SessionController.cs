using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        public SessionController(ApplicationDbContext db) => _dataBase = db;

        [HttpGet("{workoutId}", Name = "GetSessionsByWorkoutId")]
        [ProducesResponseType(typeof(IEnumerable<Session>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] int workoutId)
        {
            return Ok(_dataBase.Sessions.Where(session => session.WorkoutId == workoutId).ToList());
        }

        [HttpGet("{SessionId}", Name = "GetSessionById")]
        [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSession([FromRoute] int SessionId)
        {
            Session? session = await _dataBase.Sessions.FindAsync(SessionId);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session.CompletedSets);
        }


        [HttpGet("{SessionId}/currentExercise", Name = "GetCurrentExercise")]
        [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentExercise([FromRoute] int SessionId)
        {
            Session? session = await _dataBase.Sessions.FindAsync(SessionId);
            if (session == null)
                return NotFound("Invalid Session Number");
            var exercise = await _dataBase.Exercises.FindAsync(session.CurrentExerciseId);
            return exercise == null? NotFound() : Ok(exercise);
        }


        [HttpPatch("{sessionId}", Name = "UpdateSession")]
        public async Task<IActionResult> CompleteExercise([FromRoute] int sessionId)
        {
            Session? session = await _dataBase.Sessions.FindAsync(sessionId);
            int workoutId = session.WorkoutId;
            List<int?> exerciseList = _dataBase.ExerciseInWorkouts.Where(e => e.WorkoutId == workoutId).Select(e => e.ExerciseId).OrderBy(e => e).ToList();
            int currentExerciseIndex = exerciseList.IndexOf(session.CurrentExerciseId);
            if (currentExerciseIndex == -1)
                return BadRequest();
            if (currentExerciseIndex == exerciseList.Count - 1)
                session.Status = Status.Completed;
            else
                session.CurrentExerciseId = exerciseList[currentExerciseIndex + 1];
            _dataBase.SaveChanges();
            return Ok();
        }

        [HttpPost("{WorkoutId}", Name = "CreateSession")]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<IActionResult> Create([FromRoute] int WorkoutId)
        {
            Session session = new Session
            {
                WorkoutId = WorkoutId,
                Date = DateTime.UtcNow,
                Status = Status.InProgress
            };
            Workout? workout = await _dataBase.Workouts.FindAsync(WorkoutId);
            if (workout == null)
                return BadRequest(ModelState);
            await _dataBase.Sessions.AddAsync(session);
            await _dataBase.SaveChangesAsync();
            List<int?> exerciseList = _dataBase.ExerciseInWorkouts.Where(e => e.WorkoutId == workout.WorkoutId).Select(e => e.ExerciseId).OrderBy(e => e).ToList();
            session.CurrentExerciseId = exerciseList[0];
            Plan? plan = workout.Plan;
            var workoutList = plan.Workouts.OrderBy(workout => workout.WorkoutId).ToList();
            var currentWorkoutIndex = workoutList.IndexOf(workout);
            plan.NextWorkoutId = workoutList[(currentWorkoutIndex + 1) % workoutList.Count].WorkoutId;
            await _dataBase.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSession), new { SessionId = session.SessionId }, session);
        }
    }



}
