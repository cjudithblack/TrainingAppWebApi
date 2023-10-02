using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User(model.Name, model.Email);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optionally, sign the user in after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Ok(new { message = "Registration successful" });
                }
                else
                {
                    return BadRequest(new { message = "Registration failed", errors = result.Errors });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(model.Email).Result;
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Login successful" });
                }
                else
                {
                    return BadRequest(new { message = "Login failed" });
                }
            }

            return BadRequest(ModelState);
        }

    }

}
