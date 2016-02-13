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

        public virtual DbSet<HierarchyPost> HierarchyPosts { get; set; }

        public virtual IEnumerable<Post> PostTest => Posts.FromSql(@"WITH    cte ( Id, ParentPostId, Depth ) 
              AS ( SELECT   Id,
							ReplyToPostId,
							0 as TheLevel
			       FROM     posts
				   where ReplyToPostId is null
                   UNION ALL 
                   SELECT   pn.Id, 
                            pn.ReplyToPostId,
							p1.Depth +1
                   FROM     Posts pn
                    INNER JOIN cte AS p1 on p1.Id = pn.ReplyToPostId
                 )
select cte.Id ,ParentPostId, Depth, ForumId, LastChangedDate, PublishDate, ReplyToPostId, Text, U.UserName, u.Id as UserId
from  cte 
INNER JOIN POSTS P ON CTE.ID = P.ID
INNER JOIN USERS u ON U.id = p.UserId
order by depth
");

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
            postBuilder.HasIndex(x => new { x.PublishDate, x.LastChangedDate });
        }
    }
}
