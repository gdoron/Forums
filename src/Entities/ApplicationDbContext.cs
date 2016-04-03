using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<HierarchyPost> HierarchyPosts { get; set; }

        public virtual DbSet<Vote> Votes { get; set; }

        public DbSet<PostRevision> PostRevisions { get; set; }

        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<Message> Messages { get; set; }

        public IQueryable<HierarchyPost> GeHierarchyPost(int rootId)
        {
            return HierarchyPosts
                .Where(x => x.RootId == rootId)
                .OrderBy(x => x.ReplyToPostId ?? -1)
                .ThenByDescending(x => x.IsImportantReply)
                .ThenBy(x => x.PostId);
        }

        public IQueryable<HierarchyPost> GeHierarchyPostBySon(int postId)
        {
            return HierarchyPosts.FromSql(@"
with postsCTE as (
   select Id, ReplyToPostId
   from Posts
   where Id = @p0
   union all
   select Parent.Id, Parent.ReplyToPostId
   from Posts Parent
     join postsCTE Son on Son.ReplyToPostId = Parent.Id  -- this is the recursion
) 
select * from HierarchyPosts where RootId =(
                                            select top 1 Id
                                            from PostsCTE
                                            where ReplyToPostId is null)", postId)
                .Select(x => new HierarchyPost
                                 {
                                     Body = "not going to see this"
                                 });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.UseDbSetNamesAsTableNames(builder);


            var userBuilder = builder.Entity<ApplicationUser>();
            userBuilder.HasMany(x => x.ReviewsGiven).WithOne(x => x.FromUser).IsRequired().OnDelete(DeleteBehavior.Restrict);
            userBuilder.HasMany(x => x.ReviewsReceived).WithOne(x => x.ToUser).IsRequired().OnDelete(DeleteBehavior.Restrict);
            userBuilder.HasMany(x => x.IncomingMessages).WithOne(x => x.Recipient).IsRequired().OnDelete(DeleteBehavior.Restrict);
            userBuilder.HasMany(x => x.OutgoingMessages).WithOne(x => x.Sender).IsRequired().OnDelete(DeleteBehavior.Restrict);

            var messageBuilder = builder.Entity<Message>();
            messageBuilder.Property(x => x.SentDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
            messageBuilder.Property(x => x.Title).HasMaxLength(200);
            messageBuilder.Property(x => x.Body).HasMaxLength(1000);
            messageBuilder.HasIndex(x => new {x.IsSenderDeleted, x.SenderId});
            messageBuilder.HasIndex(x => new {x.IsRecipientDeleted, x.RecipientId});

            var userReviewBuilder = builder.Entity<UserReview>();
            userReviewBuilder.HasIndex(x => new
                                                {
                                                    x.FromUserId,
                                                    x.ToUserId
                                                }).IsUnique();

            userReviewBuilder.Property(x => x.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
            userReviewBuilder.Property(x => x.UpdateDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");

            builder.Entity<HierarchyPost>().HasKey(x => x.PostId);
            builder.Entity<Vote>().HasIndex(x => x.PostId);
            builder.Entity<Vote>().Property(x => x.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");

            var postBuilder = builder.Entity<Post>();

            postBuilder.Property(x => x.Title).HasMaxLength(400)
                // the default for string
                //.HasColumnType("nvarchar(MAX)");
                .IsRequired();
            postBuilder.HasOne(x => x.ReplyToPost).WithMany(x => x.Replies).OnDelete(DeleteBehavior.Restrict);
            postBuilder.Property(x => x.PublishDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
            postBuilder.HasIndex(x => new {x.ForumId, x.PublishDate, x.LastChangedDate});


            var postRevisionBuilder = builder.Entity<PostRevision>();
            postRevisionBuilder.Property(x => x.Title).IsRequired();
        }
    }
}