using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlanController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        private readonly UserManager<User> _userManager;
        public PlanController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _dataBase = db;
            _userManager = userManager;
        }


        [HttpGet(Name = "GetUsersPlans")]
        [ProducesResponseType(typeof(IEnumerable<Plan>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _dataBase.Plans.Where(plan => plan.UserId == userId).ToListAsync());
        }


        [HttpGet("{planId}", Name = "GetPlanById")]
        [ProducesResponseType(typeof(Plan), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlan([FromRoute] int planId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Plan? plan = await _dataBase.Plans.FirstOrDefaultAsync(plan => plan.PlanId == planId && plan.UserId == userId);
            return plan == null ? NotFound() : Ok(plan);
        }
        
        [HttpGet("currentPlan", Name = "GetCurrentPlan")]
        [ProducesResponseType(typeof(Plan), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentPlan()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = await _dataBase.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("User not found");
            if (user.CurrentPlanId == null)
                return NotFound("No plans");
            Plan? plan = await _dataBase.Plans.FindAsync(user.CurrentPlanId);
            return plan == null ? NotFound() : Ok(plan);
        }

        [HttpPost(Name = "CreatePlan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PlanAdd newPlan)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(userId).Result;
            Plan? planNameExists = _dataBase.Plans.FirstOrDefaultAsync(plan => plan.UserId == userId && plan.Name == newPlan.Name).Result;
            if (planNameExists != null)
                return BadRequest("You already have a plan with the same name");
            Plan plan = new Plan
            {
                Name = newPlan.Name,
                Description = newPlan.Description,
                User = currentUser
            };
            await _dataBase.Plans.AddAsync(plan);
            await _dataBase.SaveChangesAsync();
            if (currentUser.Plans.Count == 1) //when creating the first plan - it will be the current plan
                currentUser.CurrentPlanId = plan.PlanId;
            //_dataBase.Users.Attach(plan.User);
            await _dataBase.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlan), new { planId = plan.PlanId }, plan);
        }

        [HttpPut("Update/{id}", Name = "UpdatePlan")]
        public async Task<IActionResult> UpdatePlan([FromRoute] int id, [FromBody] PlanUpdate updatedPlan)
        {
            var plan = await _dataBase.Plans.FindAsync(id);
            if (plan != null)
            {
                plan.Name = (updatedPlan.Name != null) ? updatedPlan.Name : plan.Name;
                plan.Description = (updatedPlan.Description != null) ? updatedPlan.Description : plan.Description;
                await _dataBase.SaveChangesAsync();
                return Ok(plan);
            }
            else
                return NotFound();
        }

        [HttpDelete("Delete/{id}", Name = "DeletePlan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePlan([FromRoute] int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return BadRequest("You need to sign in in order to delete a plan");
            var plan = await _dataBase.Plans.FirstOrDefaultAsync(plan => id == plan.PlanId && userId == plan.UserId);
            if (plan != null)
            {
                _dataBase.Remove(plan);
                _dataBase.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpPatch("SetAsCurrent/{id}", Name = "SetPlanAsCurrentById")]
        public async Task<IActionResult> SetAsCurrent([FromRoute] int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User ? user = await _dataBase.Users.FindAsync(userId);
            user.CurrentPlanId = id;
            return Ok();
        }

    }
}



