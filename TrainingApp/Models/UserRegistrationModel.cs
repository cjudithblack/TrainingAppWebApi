using System.ComponentModel.DataAnnotations;

namespace TrainingApp.Models
{
    public class UserRegistrationModel
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress] 
        public string Email { get; set;}

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
