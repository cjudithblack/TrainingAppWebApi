using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Data.Migrations;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentExercise([FromRoute] int SessionId)
        {
            Session? session = await _dataBase.Sessions.FindAsync(SessionId);
            if (session == null)
            {
                return NotFound("Invalid Session Number");
            }
            return Ok(session.CurrentExerciseIndex);
        }

        [HttpPatch(Name = "CompleteSession")]
        public async Task<IActionResult> CompleteSession()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var sessionId = currentUser.CurrentSessionId;
            var session = await _dataBase.Sessions.FindAsync(sessionId);
            if (sessionId == 0 || session == null || session.Status != Status.InProgress)
                return BadRequest("no session in progress");

            session.Status = Status.Completed;
            currentUser.CurrentSessionId = 0;
            await _dataBase.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{sessionId}", Name = "CompleteExercise")]
        public async Task<IActionResult> CompleteExercise([FromRoute] int sessionId)
        {
            var session = await _dataBase.Sessions.FindAsync(sessionId);
            session.CurrentExerciseIndex++;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (session == null)
            {
                return BadRequest();
            }

            var exerciseList = _dataBase.ExerciseInWorkouts
                .Where(e => e.WorkoutId == session.WorkoutId)
                .ToList();

            if (session.CurrentExerciseIndex == exerciseList.Count)
            {
                session.Status = Status.Completed;
                currentUser.CurrentSessionId = 0;
            }

            await _dataBase.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{WorkoutId}", Name = "CreateSession")]
        [ProducesResponseType(typeof(Session), StatusCodes.Status201Created)]
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
                CurrentExerciseIndex = 0
            };

            await _dataBase.Sessions.AddAsync(session);
            await _dataBase.SaveChangesAsync();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            currentUser.CurrentSessionId = session.SessionId;
            var workoutList = workout.Plan.Workouts.OrderBy(w => w.WorkoutId).ToList();
            var currentWorkoutIndex = workoutList.IndexOf(workout);
            workout.Plan.NextWorkoutId = workoutList[(currentWorkoutIndex + 1) % workoutList.Count].WorkoutId;

            await _dataBase.SaveChangesAsync();

            return CreatedAtAction("GetSession", new { SessionId = session.SessionId }, session);
        }

    }



}
