using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Workout
    {
        public Workout()
        {
            this.ExercisesInWorkouts = new HashSet<ExerciseInWorkout>();
            this.Sessions = new HashSet<Session>();
        }
        [Key]
        public int WorkoutId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("PlanId")]
        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<ExerciseInWorkout> ExercisesInWorkouts { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
