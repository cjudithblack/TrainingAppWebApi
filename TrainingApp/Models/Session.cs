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
        [Key, Column(Order = 1)]
        public System.DateTime Date { get; set; }
        [Key, Column(Order = 2)]
        public int WorkoutId { get; set; }
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; }
        public virtual ICollection<CompletedSet> CompletedSets { get; set; }
    }
}

