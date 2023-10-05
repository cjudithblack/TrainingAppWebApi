using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Plan
    {
        public Plan() { }

        public Plan(string name, string description, User user)
        {
            this.Name = name;
            this.Description = description;
            this.Workouts = new HashSet<Workout>();
            this.User = user;
            this.UserId = user.Id;
        }
        [Key]
        public int PlanId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<Workout> Workouts { get; set; }
    }
}


