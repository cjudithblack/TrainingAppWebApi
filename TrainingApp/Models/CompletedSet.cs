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
        public int? Weight { get; set; }
        public int? Reps { get; set; }
        public string? Notes { get; set; }

        [ForeignKey("WorkoutSessionId")]
        public virtual Session? ParentWorkoutSession { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }
    }
}

