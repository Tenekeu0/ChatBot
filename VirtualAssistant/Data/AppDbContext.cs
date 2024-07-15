using Microsoft.EntityFrameworkCore;
using VirtualAssistant.Models;

namespace VirtualAssistant.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<ReponseBd> Responses { get; set; }
    }
}
