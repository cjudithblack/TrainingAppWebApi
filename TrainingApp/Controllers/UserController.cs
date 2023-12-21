using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainingApp.Data;
using TrainingApp.Models;



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
                var user = new User(model.FirstName, model.LastName, model.UserName);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await AddDefaultExercises(user, _dataBase);
                    await _dataBase.SaveChangesAsync();
                    return Ok(new { message = "Registration successful" });
                }
                else
                {
                    return BadRequest(new { message = "Registration failed", errors = result.Errors });
                }
            }
            return BadRequest(ModelState);
        }

        private async Task<IActionResult> AddDefaultExercises(User user, ApplicationDbContext dataBase)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string jsonFilePath = Path.Combine(currentDirectory, "Data\\exercises.json"); 
            
            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            List<Exercise> exercises = JsonConvert.DeserializeObject<List<Exercise>>(jsonContent);

            // Do something with the exercises...
            foreach (var exercise in exercises)
            {
                dataBase.Exercises.Add(new Exercise { 
                Name = exercise.Name,
                Instructions = exercise.Instructions,
                VideoId = exercise.VideoId,
                UserId = user.Id} );
            }
            await dataBase.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {

            if (ModelState.IsValid)
            {
                var user = _userManager.FindByNameAsync(model.UserName).Result;
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: false);
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
