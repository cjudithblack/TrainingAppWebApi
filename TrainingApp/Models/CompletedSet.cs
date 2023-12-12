using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class CompletedSet
    {
        [Key]
        public int SetId { get; set; }
        public int ExerciseId { get; set; }
        public int WorkoutSessionId { get; set; }
        public double? Weight { get; set; }
        public int? Reps { get; set; }

        [ForeignKey("WorkoutSessionId")]
        public virtual Session? ParentWorkoutSession { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }
    }
}

