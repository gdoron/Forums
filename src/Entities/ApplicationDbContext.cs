﻿using System.Linq;
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

        public IQueryable<HierarchyPost> GeHierarchyPost(int rootId)
        {
            return HierarchyPosts
                .Where(x => x.RootId == rootId)
                .OrderBy(x => x.ReplyToPostId ?? -1)
                .ThenByDescending(x => x.IsImportantReply)
                .ThenBy(x => x.PostId);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.UseDbSetNamesAsTableNames(builder);


            var userBuilder = builder.Entity<ApplicationUser>();
            userBuilder.HasMany(x => x.ReviewsGiven).WithOne(x => x.FromUser).IsRequired().OnDelete(DeleteBehavior.Restrict);
            userBuilder.HasMany(x => x.ReviewsReceived).WithOne(x => x.ToUser).IsRequired().OnDelete(DeleteBehavior.Restrict);

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

            postBuilder.Property(x => x.Title).HasMaxLength(200)
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