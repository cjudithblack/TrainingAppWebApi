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
        public User(string firstName, string lastName, string userName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;   
            this.UserName = userName;
            this.Plans = new HashSet<Plan>();
            this.Exercises = new HashSet<Exercise>();
            this.Email = "user@default.com";
        }
        public string FirstName { get; set; }
        public string LastName {  get; set; }
        public int CurrentPlanId { get; set; }
        public int CurrentSessionId { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
        public virtual ICollection<Exercise> Exercises { get; set; }
    }
}


