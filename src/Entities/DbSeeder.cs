using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (!_context.Users.Any())
            {
                await InitUsers();
            }

            if (!_context.Forums.Any())
            {
                InitForums();
            }

            if (!_context.Posts.Any())
            {
                InitPosts();
            }

            if (!_context.UserReviews.Any())
            {
                InitUserReviews();
            }

            if (!_context.Messages.Any())
            {
                InitMessages();
            }

            _context.SaveChanges();
        }

        private void InitForums()
        {
            var scoops = new Forum
                             {
                                 Name = "סקופים",
                                 Description = @"מידע חדשותי חם ובזמן אמת המובא על ידי הגולשים. חלקם עיתונאי רוטר.נט, המחוברים בזימוניות למוקדי חדשות."
                             };

            var politics = new Forum
                               {
                                   Name = "פוליטיקה ואקטואליה",
                                   Description = "מדיניות, הנהגת המדינה, אקטואליה ואזרחות"
                               };

            _context.Forums.AddRange(new List<Forum> {scoops, politics});
            _context.SaveChanges();
        }

        private async Task InitUsers()
        {
            var firstUser = new ApplicationUser
                                {
                                    Email = "test1111@test.test",
                                    UserName = "Test1111",
                                    EmailConfirmed = true
                                };

            var secondUser = new ApplicationUser
                                 {
                                     Email = "test2222@test.test",
                                     UserName = "Test2222",
                                     EmailConfirmed = true
                                 };

            var adminUser = new ApplicationUser {Email = "grdoron@gmail.com", UserName = "grdoron@gmail.com", EmailConfirmed = true};


            var adminRole = new IdentityRole
                                {
                                    Name = "Admin",
                                    NormalizedName = "ADMIN"
                                };

            _context.Roles.Add(adminRole);

            await _userManager.CreateAsync(firstUser, "Aa123456!");
            await _userManager.CreateAsync(secondUser, "Aa123456!");


            await _userManager.CreateAsync(adminUser, "Aa123456!");
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }

        private void InitPosts()
        {
            var firstUser = _context.Users.First();
            var secondUser = _context.Users.Skip(1).First();
            var var = "סקופים";
            var scoops = _context.Forums.Single(x => x.Name == var);

            for (var i = 1; i <= 30; i++)
            {
                var firstPost = new Post
                                    {
                                        Forum = scoops,
                                        Title = "Root level " + i,
                                        User = firstUser,
                                        Body = "Bla bla bla",
                                        PostType = PostType.News | PostType.Foreign
                                    };


                _context.AddRange(Enumerable.Range(1, 100).Select(x =>
                    new Post
                        {
                            Forum = scoops,
                            Title = "LLLLLLLLLOOOOOOOOOOOOOOOOOPPPPPPPPPPP! " + x,
                            Body =
                                "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO NNNNNNNNNNNNNNNNNNNNNNN GGGGGGGGGGGGGGGGGGGGGG TEXT ",
                            ReplyToPost = firstPost,
                            User = secondUser
                        }).ToList());

                var reply1 = new Post
                                 {
                                     Forum = scoops,
                                     Title = "FirstReply " + i,
                                     ReplyToPost = firstPost,
                                     User = secondUser
                                 };

                var reply2 = new Post
                                 {
                                     Forum = scoops,
                                     Title = "Second Reply " + i,
                                     ReplyToPost = firstPost,
                                     User = firstUser
                                 };

                var replytoReply2 = new Post
                                        {
                                            Forum = scoops,
                                            Title = "Reply to second Reply " + i,
                                            ReplyToPost = firstPost,
                                            User = firstUser
                                        };

                var reply3 = new Post
                                 {
                                     Forum = scoops,
                                     Title = "Third reply " + i,
                                     ReplyToPost = firstPost,
                                     User = secondUser,
                                     IsImportantReply = true
                                 };


                var reply4 = new Post
                                 {
                                     Forum = scoops,
                                     Title = "fourth reply " + i,
                                     User = secondUser,
                                     ReplyToPost = reply2
                                 };

                var closedPost = new Post
                                     {
                                         Forum = scoops,
                                         Title = "Closed post " + i,
                                         User = secondUser,
                                         IsLocked = true,
                                         LockingUser = firstUser,
                                         LockReason = "Becuase I hate you, go kill yourself",
                                         Score = 4,
                                         Votes = new List<Vote>()
                                     };

                for (var j = 0; j < 5; j++)
                {
                    closedPost.Votes.Add(new Vote
                                             {
                                                 Post = closedPost,
                                                 User = firstUser,
                                                 VoteType = VoteType.Up
                                             });
                }

                closedPost.Votes.Add(new Vote
                                         {
                                             Post = closedPost,
                                             User = firstUser,
                                             VoteType = VoteType.Down
                                         });

                _context.Posts.AddRange(firstPost, reply1, reply2, reply3, reply4, replytoReply2, closedPost);
            }
        }

        private void InitUserReviews()
        {
            var random = new Random();
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var otherUsers = users.Where(x => x.Id != user.Id).ToList();
                var goodReview = random.Next()%2 == 0;

                foreach (var otherUser in otherUsers)
                {
                    var review = new UserReview
                                     {
                                         FromUser = otherUser,
                                         ToUser = user,
                                         Review = goodReview ? "Crappy user" : "Best user ever!",
                                         VoteType = goodReview ? VoteType.Down : VoteType.Up
                                     };
                    user.ReviewsReceived.Add(review);
                }
            }
        }


        private void InitMessages()
        {
            var random = new Random();
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var otherUsers = users.Where(x => x.Id != user.Id).ToList();

                foreach (var otherUser in otherUsers)
                {
                    for (var i = 0; i < 50; i++)
                    {
                        var isRead = random.Next()%2 == 0;
                        var isRecipientDeketed = random.Next()%2 == 0;
                        var isSenderDeleted = random.Next()%2 == 0;

                        var message = new Message
                                          {
                                              Recipient = otherUser,
                                              Sender = user,
                                              Title = $"Private message {i}",
                                              Body = $"Private message body {i}",
                                              IsRead = isRead,
                                              IsRecipientDeleted = isRecipientDeketed,
                                              IsSenderDeleted = isSenderDeleted
                                          };
                        _context.Messages.Add(message);
                    }
                }
            }
        }
    }
}