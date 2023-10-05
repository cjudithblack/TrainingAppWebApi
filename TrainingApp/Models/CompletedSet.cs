using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class CompletedSet
    {
        [Key]
        public int SetId { get; set; }
        public int ExerciseId { get; set; }
        public System.DateTime Date { get; set; }
        public int WorkoutId { get; set; }
        public int? Weight { get; set; }
        public int? Reps { get; set; }

        [ForeignKey("ParentWorkoutSession")]
        public virtual Session? ParentWorkoutSession { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }
    }
}

