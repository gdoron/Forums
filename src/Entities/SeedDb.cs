using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Forums.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Entities
{
    public static class ApplicationDbContextExtension
    {
        public static void EnsureSeedData(this ApplicationDbContext context)
        {
            if (!context.Posts.Any())
            {
                var firstUser = new ApplicationUser
                                    {
                                        Email = "test1111@test.test",
                                        UserName = "Test1111"
                                    };

                var secondUser = new ApplicationUser
                                     {
                                         Email = "test222@test.test",
                                         UserName = "Test2222"
                                     };
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
                                        PublishDate = DateTime.Now,
                                        User = firstUser
                                    };
                var secondPost = new Post
                                     {
                                         Forum = newsForum,
                                         Text = "Second post",
                                         PublishDate = DateTime.Now,
                                         ReplyToPost = firstPost,
                                         User = secondUser
                                     };

                var thirdPost = new Post
                                    {
                                        Forum = newsForum,
                                        Text = "third post",
                                        PublishDate = DateTime.Now,
                                        ReplyToPost = firstPost,
                                        User = firstUser
                                    };

                var post4 = new Post
                                {
                                    Forum = newsForum,
                                    Text = "Child to 3",
                                    PublishDate = DateTime.Now,
                                    ReplyToPost = thirdPost,
                                    User = secondUser
                                };

                var moreRoot = new Post
                                   {
                                       Forum = newsForum,
                                       Text = "Another root",
                                       PublishDate = DateTime.Now,
                                       User = firstUser
                                   };
                context.Users.AddRange(firstUser, secondUser);
                context.Posts.AddRange(firstPost, secondPost, thirdPost, post4, moreRoot);
                var adminRole = new IdentityRole
                                    {
                                        Name = "Admin",
                                        NormalizedName = "Admin"
                                    };
                context.Roles.Add(adminRole);

                //var adminUsers = new ApplicationUser()
                //                     {
                //                         Email = "doron@jifiti.com",
                //                         UserName = "Doron jifiti"
                //                     };

                context.SaveChanges();
            }
        }

        public static void UseDbSetNamesAsTableNames(this DbContext dbContext, ModelBuilder modelBuilder)
        {
            var dbSets = dbContext.GetType().GetRuntimeProperties()
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