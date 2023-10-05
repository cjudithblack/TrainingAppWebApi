using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    [PrimaryKey(nameof(Date), nameof(WorkoutId))]
    public class Session
    {
        public Session()
        {
            this.CompletedSets = new HashSet<CompletedSet>();
        }
        [Key]
        public int SessionId;
        public System.DateTime Date { get; set; }
        public int WorkoutId { get; set; }
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; }
        public virtual ICollection<CompletedSet> CompletedSets { get; set; }
    }
}

