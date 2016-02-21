using System.Collections.Generic;
using System.Linq;
using Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Forums.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<HierarchyPost> HierarchyPosts { get; set; }

        public IQueryable<HierarchyPost> GeHierarchyPost(int rootId)
        {
            return HierarchyPosts
                .Where(x => x.RootId == rootId)
                .OrderBy(x => x.ReplyToPostId ?? -1)
                .ThenByDescending(x => x.IsImportantReply)
                .ThenBy(x => x.PostId);
        } 

        //public IQueryable<HierarchyPost> A => HierarchyPosts.FromSql("select * from HierarchyPosts order by IsNull(ReplyToPostId,-1), isImportantReply desc, PostId");
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.UseDbSetNamesAsTableNames(builder);

            builder.Entity<HierarchyPost>().HasKey(x => x.PostId);


            var postBuilder = builder.Entity<Post>();
            // need to create Default value for this to work. 
            //postBuilder.Property(x => x.PublishDate).ValueGeneratedOnAdd();

            postBuilder.Property(x => x.Text)
                // the default for string
                //.HasColumnType("nvarchar(MAX)");
                .IsRequired();
            postBuilder.HasOne(x => x.ReplyToPost).WithMany(x => x.Replies).OnDelete(DeleteBehavior.Restrict);
            postBuilder.Property(x => x.PublishDate).HasDefaultValueSql("getdate()");
            postBuilder.Property(x => x.LastChangedDate).ValueGeneratedOnAddOrUpdate();
            postBuilder.HasIndex(x => new { x.PublishDate, x.LastChangedDate });
        }
    }
}
