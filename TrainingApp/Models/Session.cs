﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingApp.Models
{
    public enum Status
    {
        InProgress,
        Completed
    }
    public class Session
    {
        public Session()
        {
            this.CompletedSets = new HashSet<CompletedSet>();
        }

        [Key]
        public int SessionId { get; set; }
        public System.DateTime Date { get; set; }
        public Status Status { get; set; }
        public int CurrentExerciseIndex { get; set; }
        public int WorkoutId { get; set; }
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; }
        public virtual ICollection<CompletedSet> CompletedSets { get; set; }
    }
}

