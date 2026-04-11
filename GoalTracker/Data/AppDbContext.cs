using GoalTracker.Models;
using Microsoft.EntityFrameworkCore;
using GoalTask = GoalTracker.Models.GoalTask;

namespace GoalTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalTask> GoalTasks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Goal>().
                Property(g => g.UrgencyLevel)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
