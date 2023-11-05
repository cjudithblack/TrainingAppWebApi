namespace TrainingApp.Models
{
    public class GroupedSetResponse
    {
        public int ExerciseId { get; set; }
        public List<CompletedSet> Sets { get; set; }
    }
}

