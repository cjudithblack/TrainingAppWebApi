using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainingApp.Data;
using TrainingApp.Models;
using static System.Net.WebRequestMethods;

namespace TrainingApp.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dataBase;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager, 
            ApplicationDbContext database)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataBase = database;
        }

        [HttpPost("register", Name = "Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User(model.FirstName, model.LastName, model.Email);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    AddDefaultExercises(user, _dataBase);
                    return Ok(new { message = "Registration successful" });
                }
                else
                {
                    return BadRequest(new { message = "Registration failed", errors = result.Errors });
                }
            }
            return BadRequest(ModelState);
        }

        private async void AddDefaultExercises(User user, ApplicationDbContext dataBase)
        {
            List<Exercise> defaultExerciseList = new List<Exercise>()
            {
                new Exercise{Name = "Squats"     ,   Description = "Bend your knees and lower your body as if you're sitting down. Keep your back straight and your chest up. Push through your heels as you return to a standing position. Great for building leg strength and improving posture." ,
                             UserId = user.Id    ,   VideoUrl = "<iframe width=\"320\" height=\"560\" src=\"https://www.youtube.com/embed/AIZ8q1qruKw\" title=\"How to Perform a PERFECT Squat\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen></iframe>" },
                new Exercise{Name = "Push-ups"   ,   Description = "Start in a plank position, lower your body by bending your elbows, and then push back up. Keep your body straight throughout the movement. Excellent for strengthening your chest, arms, and core.",
                             UserId = user.Id    ,   VideoUrl = "<iframe width=\"320\" height=\"560\" src=\"https://www.youtube.com/embed/ATink4Ix84A\" title=\"💪🏽 6 PUSH UP VARIATIONS TO BUILD A STRONGER CHEST, TRICEPS, SHOULDERS &amp; BACK 🔥\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen></iframe>" },
                new Exercise{Name = "Lunges"     ,   Description = "Step forward with one leg and lower your body until your front thigh is parallel to the ground. Push back up to the starting position. Repeat with the other leg. Helps build leg strength and improve balance."                ,
                             UserId = user.Id    ,   VideoUrl = "<iframe width=\"320\" height=\"560\" src=\"https://www.youtube.com/embed/kn431INOxig\" title=\"How To Do Lunges: Lunge Progression Exercises\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen></iframe>" },
                new Exercise{Name = "Plank"      ,   Description = "Hold a position similar to the top of a push-up, with your body forming a straight line from head to heels. Engage your core muscles and hold for as long as you can. Effective for building core strength and stability."      ,
                             UserId = user.Id    ,   VideoUrl = "<iframe width=\"320\" height=\"560\" src=\"https://www.youtube.com/embed/wZo1k2-3Zn4\" title=\"How to Do a Plank Safely\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen></iframe>" },
                new Exercise{Name = "Bicep Curls",   Description = "Hold a dumbbell in each hand, palms facing up. Bend your elbows and curl the weights toward your shoulders, then lower them back down. A great way to strengthen and tone your biceps."                                         ,
                             UserId = user.Id    ,   VideoUrl = "<iframe width=\"320\" height=\"560\" src=\"https://www.youtube.com/embed/09AYfVFf7pg\" title=\"Dumbbell bicep curls\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen></iframe>" },
};
            foreach (var exercise in defaultExerciseList)
            {
                dataBase.Exercises.Add(exercise);
            }
            await dataBase.SaveChangesAsync();
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {

            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(model.Email).Result;
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Create claims for the user
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyHere"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "http://localhost:3000/",
                        audience: "http://localhost:3000/",
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(200), //"never expires"
                        signingCredentials: creds
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new { message = "Login successful", token = tokenString });

                }
                else
                {
                    return BadRequest(new { message = "Login failed" });
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet(Name = "GetCurrentUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logout successful" });
        }

    }

}
