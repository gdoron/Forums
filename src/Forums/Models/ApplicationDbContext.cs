using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Forums.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Post> Posts  { get; set; }
        public virtual DbSet<Forum> Forums  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Forum>()
                .HasMany(x => x.Posts);

            var postBuilder = builder.Entity<Post>();
            // need to create Default value for this to work. 
            //postBuilder.Property(x => x.PublishDate).ValueGeneratedOnAdd();

            postBuilder.Property(x => x.Text).HasColumnType("nvarchar(MAX)");
            postBuilder.HasOne(x => x.Forum).WithMany(x => x.Posts);
            

        }
    }
}
