using System;

namespace Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public VoteType VoteType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}