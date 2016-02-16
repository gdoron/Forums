﻿using System;

namespace Entities
{
    public class HierarchyPost
    {
        public int PostId { get; set; }
        public int ParentPostId { get; set; }
        public int Depth { get; set; }
        public int ForumId { get; set; }
        public DateTime? LastChangedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public int? ReplyToPostId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}