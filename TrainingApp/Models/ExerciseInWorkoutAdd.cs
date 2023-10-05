namespace TrainingApp.Models
{
    public class ExerciseInWorkoutAdd
    {
        public int NumOfSets { get; set; }
        public int NumOfReps { get; set; }
        public TimeSpan? RestTime { get; set; }
    }
}