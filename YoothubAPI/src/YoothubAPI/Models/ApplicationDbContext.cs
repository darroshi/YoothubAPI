using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace YoothubAPI.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Song> Songs { get; set; }

        public DbSet<Vote> Votes { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<SongTag> SongTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Song>()
                .HasIndex(s => s.SongId)
                .IsUnique();
        }

        // This method connects the context with the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=db;Port=5432;Username=postgres;Password=abc123;Database = yoothub;");
        }
    }
}
