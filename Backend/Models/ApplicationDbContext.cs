
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TabsDb> Tabs { get; set; }
        public DbSet<CategoryDb> Category { get; set; }
    }
}
