using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public bool IsModified { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ForumId { get; set; }

        public virtual Forum Forum { get; set; }

        public int? ReplyToPostId { get; set; }

        public Post ReplyToPost { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsImportantReply { get; set; }

        public virtual List<Post> Replies { get; set; }

        public List<PostType> PostTypes { get; set; }
    }
}
