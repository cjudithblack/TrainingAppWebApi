using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        private readonly UserManager<User> _userManager;
        public SessionController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _dataBase = db;
            _userManager = userManager;
        }

        [HttpGet("Workout/{workoutId}", Name = "GetSessionsByWorkoutId")]
        [ProducesResponseType(typeof(IEnumerable<Session>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] int workoutId)
        {
            return Ok(await _dataBase.Sessions
                .Include(s => s.Workout)
                .Where(s => s.WorkoutId == workoutId)
                .ToListAsync());
        }

        [HttpGet(Name = "GetUsersSessions")]
        [ProducesResponseType(typeof(IEnumerable<Session>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsersSessions()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users
                .Include(u => u.Plans)
                    .ThenInclude(p => p.Workouts)
                        .ThenInclude(w => w.Sessions)
                        .ThenInclude(s => s.Workout)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var usersSessions = currentUser?.Plans
                .SelectMany(plan => plan.Workouts.SelectMany(workout => workout.Sessions))
                .ToList();

            return Ok(usersSessions);
        }


        [HttpGet("Session/{SessionId}", Name = "GetSessionById")]
        [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSession([FromRoute] int SessionId)
        {
            Session? session = await _dataBase.Sessions
                .Include(s => s.CompletedSets)
                .Include(s => s.Workout)
                .FirstOrDefaultAsync(s => s.SessionId == SessionId);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }


        [HttpGet("{SessionId}/currentExercise", Name = "GetCurrentExercise")]
        [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentExercise([FromRoute] int SessionId)
        {
            Session? session = await _dataBase.Sessions.FindAsync(SessionId);
            if (session == null)
            {
                return NotFound("Invalid Session Number");
            }

            var exercise = await _dataBase.Exercises
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExerciseId == session.CurrentExerciseId);

            return exercise == null ? NotFound() : Ok(exercise);
        }


        [HttpPatch("{sessionId}", Name = "CompleteExercise")]
        public async Task<IActionResult> CompleteExercise([FromRoute] int sessionId)
        {
            var session = await _dataBase.Sessions.FindAsync(sessionId);

            if (session == null)
            {
                return BadRequest();
            }

            var exerciseList = _dataBase.ExerciseInWorkouts
                .Where(e => e.WorkoutId == session.WorkoutId)
                .OrderBy(e => e.ExerciseId)
                .Select(e => e.ExerciseId)
                .ToList();

            var currentExerciseIndex = exerciseList.IndexOf(session.CurrentExerciseId);

            if (currentExerciseIndex == -1)
            {
                return BadRequest();
            }

            if (currentExerciseIndex == exerciseList.Count - 1)
            {
                session.Status = Status.Completed;
            }
            else
            {
                session.CurrentExerciseId = exerciseList[currentExerciseIndex + 1];
            }

            await _dataBase.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{WorkoutId}", Name = "CreateSession")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromRoute] int WorkoutId)
        {
            var workout = await _dataBase.Workouts
                .Include(w => w.Plan)
                .ThenInclude(p => p.Workouts)
                .FirstOrDefaultAsync(w => w.WorkoutId == WorkoutId);

            if (workout == null)
            {
                return BadRequest(ModelState);
            }

            var session = new Session
            {
                WorkoutId = WorkoutId,
                Date = DateTime.UtcNow,
                Status = Status.InProgress,
                CurrentExerciseId = _dataBase.ExerciseInWorkouts
                    .Where(e => e.WorkoutId == WorkoutId)
                    .OrderBy(e => e.ExerciseId)
                    .Select(e => e.ExerciseId)
                    .FirstOrDefault()
            };

            _dataBase.Sessions.Add(session);

            var workoutList = workout.Plan.Workouts.OrderBy(w => w.WorkoutId).ToList();
            var currentWorkoutIndex = workoutList.IndexOf(workout);
            workout.Plan.NextWorkoutId = workoutList[(currentWorkoutIndex + 1) % workoutList.Count].WorkoutId;

            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSession), new { SessionId = session.SessionId }, session);
        }

    }



}
