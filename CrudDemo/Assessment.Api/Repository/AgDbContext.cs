using Assessment.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Repository
{
    public class AgDbContext : DbContext
    {
        public AgDbContext(DbContextOptions<AgDbContext> options) : base(options)
        {
        }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
