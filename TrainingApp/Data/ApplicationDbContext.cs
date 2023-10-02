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
        //public DbSet<User> Users { get; set; }
        public DbSet<CompletedSet> CompletedSets { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseInWorkout> ExerciseInWorkouts { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Workout> Workouts { get; set; }


        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workout>()
                .HasMany(e => e.ExercisesInWorkouts)
                .WithOne(e => e.Workout)
                .HasForeignKey(e => e.WorkoutId);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.ExerciseInWorkouts)
                .WithOne(e => e.Exercise)
                .HasForeignKey(e => e.ExerciseId);

            modelBuilder.Entity<Session>()
                 .HasMany(e => e.CompletedSets)
                 .WithOne(e => e.ParentWorkoutSession);
                 //.HasForeignKey(e => e.WorkoutId);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.CompletedSets)
                .WithOne(e => e.Exercise)
                .HasForeignKey(e => e.ExerciseId);

            modelBuilder.Entity<Workout>()
                .HasMany(e => e.Sessions)
                .WithOne(e => e.Workout)
                .HasForeignKey(e => e.WorkoutId);
        }
    */
    }
}
