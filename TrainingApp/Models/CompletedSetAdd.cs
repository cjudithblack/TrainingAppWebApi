using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public class CompletedSetAdd
    {
        public double? Weight { get; set; }
        public int? Reps { get; set; }

    }
}

