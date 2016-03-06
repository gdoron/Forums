using System;

namespace Entities
{
    public class UserReview
    {
        public int Id { get; set; }
        public string Review { get; set; }
        public string FromUserId { get; set; }
        public ApplicationUser FromUser { get; set; }
        public string ToUserId{ get; set; }
        public ApplicationUser ToUser { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate{ get; set; }
        public bool IsDeleted { get; set; }
    }
}