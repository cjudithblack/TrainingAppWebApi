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
    public class PlanController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;
        private readonly UserManager<User> _userManager;
        public PlanController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _dataBase = db;
            _userManager = userManager;
        }


        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Plan>> Get()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _dataBase.Plans.Where(plan => plan.UserId == userId).ToListAsync();
        }


        [HttpGet("{planId}")]
        [ProducesResponseType(typeof(Plan), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getPlan(int planId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var plan = await _dataBase.Plans.FirstOrDefaultAsync(plan => plan.PlanId == planId && plan.UserId == userId);
            return plan == null ? NotFound() : Ok(plan);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] string Name, string Description)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(userId).Result;
            Plan plan = new Plan(Name, Description, currentUser);
            await _dataBase.Plans.AddAsync(plan);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(getPlan), new { planId = plan.PlanId }, plan);
        }
    }
}



