using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class Exercise
    {
        public Exercise()
        {
            this.ExerciseInWorkouts = new HashSet<ExerciseInWorkout>();
            this.CompletedSets = new HashSet<CompletedSet>();
        }
        [Key]
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public string VideoId { get; set; }
        public int? LastWeight { get; set; }
        public virtual ICollection<ExerciseInWorkout> ExerciseInWorkouts { get; set; }
        public virtual ICollection<CompletedSet> CompletedSets { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}

