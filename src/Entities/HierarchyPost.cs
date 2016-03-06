using System;

namespace Entities
{
    public class HierarchyPost
    {
        public int PostId { get; set; }
        public int? ReplyToPostId { get; set; }
        public int Depth { get; set; }
        public int ForumId { get; set; }
        public DateTime? LastChangedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int RootId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsImportantReply { get; set; }
        public int PositiveReviewsScore { get; set; }
        public int NegativeReviewsScore { get; set; }
    }
}