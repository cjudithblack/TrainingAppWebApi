﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        public SessionController(ApplicationDbContext db) => _dataBase = db;

        [HttpGet("{workoutId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Plan>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] int workoutId)
        {
            return Ok(_dataBase.Sessions.Where(session => session.WorkoutId == workoutId).ToList());
        }

        [HttpGet("{SessionId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Plan>), StatusCodes.Status200OK)]
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

        [HttpPost("{WorkoutId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<IActionResult> Create([FromRoute] int WorkoutId)
        {
            Session session = new Session
            {
                WorkoutId = WorkoutId,
                Date = DateTime.UtcNow
            };
            Workout? workout = await _dataBase.Workouts.FindAsync(WorkoutId);
            if (workout == null)
                return BadRequest(ModelState);
            await _dataBase.Sessions.AddAsync(session);
            await _dataBase.SaveChangesAsync();
            Plan? plan = workout.Plan;
            var workoutList = plan.Workouts.OrderBy(workout => workout.WorkoutId).ToList();
            var currentWorkoutIndex = workoutList.IndexOf(workout);
            plan.NextWorkoutId = workoutList[(currentWorkoutIndex + 1) % workoutList.Count].WorkoutId;
            return CreatedAtAction(nameof(GetSession), new { SessionId = session.SessionId }, session);
        }
    }



}
