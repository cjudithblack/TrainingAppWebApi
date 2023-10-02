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
        public Nullable<int> Weight { get; set; }
        public Nullable<int> Reps { get; set; }

        public virtual Session? ParentWorkoutSession { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }
    }
}

