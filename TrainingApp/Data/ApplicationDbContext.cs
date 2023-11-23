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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompletedSet>()
                .HasOne(p => p.Exercise)
                .WithMany(b => b.CompletedSets)
                .HasForeignKey(p => p.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(p => p.Workout)
                .WithMany(b => b.Sessions)
                .HasForeignKey(p => p.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception is due to a foreign key constraint violation
                if (ex.InnerException is System.Data.SqlClient.SqlException sqlException
                    && sqlException.Number == 547)
                {
                    // Foreign key constraint violation
                    throw new DbUpdateException("Cannot delete this record because it is referenced by other records.", ex);
                }

                throw;
            }
        }
    }
}
