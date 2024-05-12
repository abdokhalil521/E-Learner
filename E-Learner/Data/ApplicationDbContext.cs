using E_Learner.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Learner.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {

        }
        public DbSet<Student> student { get; set; }
        public DbSet<UnVerified> unverified { get; set; }
    }
}
