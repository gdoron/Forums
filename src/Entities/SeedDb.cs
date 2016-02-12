using System;
using System.Collections.Generic;
using Forums.Models;
using System.Linq;
using Microsoft.Data.Entity;

namespace Entities
{
    public static class ApplicationDbContextExtension
    {
        public static void EnsureSeedData(this ApplicationDbContext context)
        {
            if (!context.Posts.Any())
            {
                var newsForum = new Forum
                                    {
                                        Name = "News",
                                        Description = "News forum"
                                    };
                var sportForum = new Forum
                                     {
                                         Name = "Sport",
                                         Description = "Sport forum"
                                     };
                context.Forums.AddRange(new List<Forum> {newsForum, sportForum});


                var firstPost = new Post
                                    {
                                        Forum = newsForum,
                                        Text = "Root level",
                                        PublishDate = DateTime.Now

                                    };
                var secondPost = new Post
                                     {
                                         Forum = newsForum,
                                         Text = "Second post",
                                         PublishDate = DateTime.Now,
                                         ReplyToPost = firstPost
                                     };

                var thirdPost = new Post
                                    {
                                        Forum = newsForum,
                                        Text = "third post",
                                        PublishDate = DateTime.Now,
                                        ReplyToPost = firstPost
                                    };

                var post4 = new Post
                                {
                                    Forum = newsForum,
                                    Text = "Child to 3",
                                    PublishDate = DateTime.Now,
                                    ReplyToPost = thirdPost
                                };

                var moreRoot = new Post
                                   {
                                       Forum = newsForum,
                                       Text = "Another root",
                                       PublishDate = DateTime.Now
                                   };

                context.Posts.AddRange(firstPost, secondPost, thirdPost, post4, moreRoot);
                context.SaveChanges();
            }
        }

        public static void UseDbSetNamesAsTableNames(this DbContext dbContext, ModelBuilder modelBuilder)
        {

            var dbSets = dbContext.GetType().GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => new
                                 {
                                     PropertyName = p.Name,
                                     EntityType = p.PropertyType.GenericTypeArguments.Single()
                                 })
                .ToArray();

            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                var dbset = dbSets.SingleOrDefault(s => s.EntityType == type.ClrType);
                if (dbset != null)
                {
                    type.Relational().TableName = dbset.PropertyName;
                }

            }
        }
    }
}