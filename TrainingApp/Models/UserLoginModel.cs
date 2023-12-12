using System.ComponentModel.DataAnnotations;

namespace TrainingApp.Models
{
    public class UserLoginModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

}
