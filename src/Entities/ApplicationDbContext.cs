using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;

namespace Forums.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.UseDbSetNamesAsTableNames(builder);


            var postBuilder = builder.Entity<Post>();
            // need to create Default value for this to work. 
            //postBuilder.Property(x => x.PublishDate).ValueGeneratedOnAdd();

            postBuilder.Property(x => x.Text)
                // the default for string
                //.HasColumnType("nvarchar(MAX)");
                .IsRequired();
            postBuilder.HasOne(x => x.ReplyToPost).WithMany(x => x.Replies).OnDelete(DeleteBehavior.Restrict);
            postBuilder.Property(x => x.PublishDate).HasDefaultValueSql("getdate()");
            postBuilder.HasIndex(x => new {x.PublishDate, x.LastChangedDate});
        }
    }
}
