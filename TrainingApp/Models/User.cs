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
        public User(string firstName, string lastName, string email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;   
            this.UserName = firstName + " " + lastName;
            this.Email = email;
            this.Plans = new HashSet<Plan>();
            this.Exercises = new HashSet<Exercise>();
        }
        public string FirstName { get; set; }
        public string LastName {  get; set; }
        public int CurrentPlanId { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
        public virtual ICollection<Exercise> Exercises { get; set; }
    }
}


