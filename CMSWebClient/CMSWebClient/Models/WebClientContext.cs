using Microsoft.EntityFrameworkCore;
using CMSWebClient.Models;

namespace CMSWebClient.Models
{
    public class WebClientContext : DbContext
    {
        public WebClientContext(DbContextOptions<WebClientContext> options) : base(options)
        {}

        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("cam_cms");

        }
    }
}