using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class CompletedSetAdd
    {
        public int? Weight { get; set; }
        public int? Reps { get; set; }
        public string? Notes { get; set; }

    }
}

