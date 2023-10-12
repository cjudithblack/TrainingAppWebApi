using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class User : IdentityUser
    {

        public User() {
            this.Plans = new HashSet<Plan>();
            this.Exercises = new HashSet<Exercise>();
        }
        public User(string name, string email)
        {
            this.UserName = name;
            this.Email = email;
            this.Plans = new HashSet<Plan>();
            this.Exercises = new HashSet<Exercise>();
        }
        public int CurrentPlanId { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
        public virtual ICollection<Exercise> Exercises { get; set; }
    }
}


