namespace TrainingApp.Models
{
    public class ExerciseInWorkoutUpdate
    {
        public int NumOfSets { get; set; }
        public int NumOfReps { get; set; }
        public TimeSpan? RestTime { get; set; }
    }
}