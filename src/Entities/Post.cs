﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public int ForumId { get; set; }

        public virtual Forum Forum { get; set; }

        public int? ReplyToPostId { get; set; }

        public Post ReplyToPost { get; set; }

        public virtual List<Post> Replies { get; set; }
    }
}
