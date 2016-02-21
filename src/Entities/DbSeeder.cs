using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forums.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Entities
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task EnsureSeedData()
        {
            if (!_context.Posts.Any())
            {
                var adminRole = new IdentityRole
                                    {
                                        Name = "Admin",
                                        NormalizedName = "ADMIN"
                                    };
                _context.Roles.Add(adminRole);

                var firstUser = new ApplicationUser
                                    {
                                        Email = "test1111@test.test",
                                        UserName = "Test1111",
                                        EmailConfirmed = true
                                    };

                var secondUser = new ApplicationUser
                                     {
                                         Email = "test222@test.test",
                                         UserName = "Test2222",
                                         EmailConfirmed = true
                                     };


                await _userManager.CreateAsync(firstUser, "Aa123456!");
                await _userManager.CreateAsync(secondUser, "Aa123456!");

                var adminUser = new ApplicationUser {Email = "grdoron@gmail.com", UserName = "grdoron@gmail.com", EmailConfirmed = true};

                await _userManager.CreateAsync(adminUser, "Aa123456!");
                await _userManager.AddToRoleAsync(adminUser, "Admin");


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
                _context.Forums.AddRange(new List<Forum> {newsForum, sportForum});


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
                _context.Posts.AddRange(firstPost, secondPost, thirdPost, post4, moreRoot);

                //var adminUsers = new ApplicationUser()
                //                     {
                //                         Email = "doron@jifiti.com",
                //                         UserName = "Doron jifiti"
                //                     };

                _context.SaveChanges();
            }
        }
    }
}