namespace TrainingApp.Models
{
    public class GroupedSetResponse
    {
        public Exercise Exercise { get; set; }
        public List<CompletedSet> Sets { get; set; }
    }
}

