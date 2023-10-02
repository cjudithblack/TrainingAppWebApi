using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class User : IdentityUser
    {
        private static int currentId = 1;

        public User() {
            this.Id = currentId++.ToString();
        }
        public User(string name, string email)
        {
            this.Id = currentId++.ToString();
            this.UserName = name;
            this.Email = email;
            this.Plans = new HashSet<Plan>();
            this.Exercises = new HashSet<Exercise>();
        }
        public virtual ICollection<Plan> Plans { get; set; }
        public virtual ICollection<Exercise> Exercises { get; set; }
    }
}


