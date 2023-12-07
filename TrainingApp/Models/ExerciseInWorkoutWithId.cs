namespace TrainingApp.Models
{
    public class ExerciseInWorkoutWithId
    {
        public int exerciseId {  get; set; }
        public int NumOfSets { get; set; }
        public int NumOfReps { get; set; }
        public TimeSpan? RestTime { get; set; }
        public string? Notes { get; set; }

    }
}
