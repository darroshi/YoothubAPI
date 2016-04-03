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
        public ApplicationDbContext()
        {
            this.Database.EnsureCreated();
        }

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
    }
}
