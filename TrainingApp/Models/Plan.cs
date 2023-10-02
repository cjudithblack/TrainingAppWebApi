using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Plan
    {
        public Plan()
        {
            this.Workouts = new HashSet<Workout>();
        }
        [Key]
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<Workout> Workouts { get; set; }
    }
}


