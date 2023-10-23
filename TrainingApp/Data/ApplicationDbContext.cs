using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using TrainingApp.Models;

namespace TrainingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        public DbSet<CompletedSet> CompletedSets { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseInWorkout> ExerciseInWorkouts { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Workout> Workouts { get; set; }


    }
}
