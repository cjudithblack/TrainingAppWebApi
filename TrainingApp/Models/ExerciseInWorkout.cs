using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    [PrimaryKey(nameof(WorkoutId), nameof(ExerciseId))]
    public class ExerciseInWorkout
    {
        [Key, Column(Order = 1)]
        public int? WorkoutId { get; set; }
        [Key, Column(Order = 2)]
        public int? ExerciseId { get; set; }

        public int NumOfSets { get; set; }
        public int NumOfReps { get; set; }
        public Nullable<System.TimeSpan> RestTime { get; set; }
        public string? Notes { get; set; }
        [ForeignKey("WorkoutId")]
        public virtual Workout? Workout { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }
    }
}
