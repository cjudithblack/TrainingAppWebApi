﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExerciseController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;

        public ExerciseController(ApplicationDbContext db) => _dataBase = db;


        [HttpGet(Name = "GetUsersExercises")]
        [ProducesResponseType(typeof(IEnumerable<Exercise>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Exercise> exercises = await _dataBase.Exercises
                .Where(exercise => exercise.UserId == userId)
                .ToListAsync();
            if (exercises.Count == 0)
                return NotFound();
            return Ok(exercises);
        }


        [HttpGet("{exerciseId}", Name = "GetExerciseById")]
        [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExercise([FromRoute] int exerciseId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exercise = await _dataBase.Exercises
                .Include(e => e.User)
                .FirstOrDefaultAsync(exercise => exercise.ExerciseId == exerciseId && exercise.UserId == userId);
            return exercise == null ? NotFound() : Ok(exercise);
        }

        [HttpPost("CreateExercise", Name = "CreateExercise")]
        [ProducesResponseType(typeof(Exercise), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] ExerciseInfo newExercise)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Exercise? exerciseNameExists = _dataBase.Exercises.FirstOrDefaultAsync(e => e.UserId == userId && e.Name == newExercise.Name).Result;
            if (exerciseNameExists != null)
                return BadRequest("You already have an exercise with the same name");
            Exercise exercise = new Exercise
            {
                Name = newExercise.Name,
                Instructions = newExercise.Instructions,
                VideoId = newExercise.VideoId,
                UserId = userId
            };
            await _dataBase.Exercises.AddAsync(exercise);
            await _dataBase.SaveChangesAsync();
            return CreatedAtAction("GetExercise", new { exerciseId = exercise.ExerciseId }, exercise);
        }


        [HttpPut("UpdateExercise/{id}", Name = "UpdateExercise")]
        public async Task<IActionResult> UpdateExercise([FromRoute] int id, [FromBody] ExerciseInfo updatedExercise)
        {
            var exercise = await _dataBase.Exercises.FindAsync(id);
            if (exercise != null)
            {
                exercise.Name = updatedExercise.Name;
                exercise.Instructions = updatedExercise.Instructions;
                exercise.VideoId = updatedExercise.VideoId;
                await _dataBase.SaveChangesAsync();
                return Ok(exercise);
            }
            else
                return NotFound();
        }

        [HttpDelete("Delete/{id}", Name = "DeleteExercise")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExercise([FromRoute] int id)
        {
            var exercise = await _dataBase.Exercises.FindAsync(id);
            try
            {
                if (exercise != null)
                {
                    _dataBase.Remove(exercise);
                    _dataBase.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException sqlException
                    && sqlException.Number == 547)
                {
                    // Foreign key constraint violation
                    return BadRequest("Cannot delete this exercise because it is referenced by other records.");
                }

                // Handle other types of DbUpdateException or rethrow if necessary
                return StatusCode(500, "Cannot delete this record because it is referenced by other records.");
            }
        }

    }

}
