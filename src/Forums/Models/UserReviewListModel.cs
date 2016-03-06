using System;
using Entities;

namespace Forums.Models
{
    public class UserReviewListModel
    {
        public int Id { get; set; }
        public string Review { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}