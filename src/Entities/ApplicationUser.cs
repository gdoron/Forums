using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //public int TotalReviewsUp { get; set; }
        //public int TotalReviewsDown { get; set; }
        public List<UserReview> ReviewsGiven{ get; set; }
        public List<UserReview> ReviewsReceived { get; set; }
        public List<Message> IncomingMessages { get; set; }
        public List<Message> OutgoingMessages { get; set; }

        public ApplicationUser()
        {
            ReviewsGiven = new List<UserReview>();
            ReviewsReceived = new List<UserReview>();
            IncomingMessages = new List<Message>();
            OutgoingMessages = new List<Message>();
        }
    }
}
