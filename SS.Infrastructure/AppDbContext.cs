
using Microsoft.EntityFrameworkCore;
using SS.Core.Entities;

namespace SS.Infrastructure
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<JobOpportunity> JobOpportunities { get; set; }
        public DbSet<ApplyJob> ApplyJobs { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
 