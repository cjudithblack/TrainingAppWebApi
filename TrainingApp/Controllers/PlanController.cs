using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Data;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly ApplicationDbContext _dataBase;

        public PlanController(ApplicationDbContext db) => _dataBase = db;


        [HttpGet("{UserId}")]
        public async Task<IEnumerable<Plan>> Get(int userId)
        {
            return await _dataBase.Plans.Where(plan => plan.UserId == userId).ToListAsync();
        }


        [HttpGet("{userId}/{planId}")]
        [ProducesResponseType(typeof(Plan), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getPlan(int userId, int planId)
        {
            var plan = await _dataBase.Plans.FirstOrDefaultAsync(plan => plan.PlanId == planId && plan.UserId == userId);
            return plan == null ? NotFound() : Ok(plan);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Plan plan)
        {
            await _dataBase.Plans.AddAsync(plan);
            await _dataBase.SaveChangesAsync();

            return CreatedAtAction(nameof(getPlan), new { id = plan.PlanId }, plan);
        }
    }
}



