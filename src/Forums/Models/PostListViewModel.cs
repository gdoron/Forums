using System;
using Entities;

namespace Forums.Models
{
    public class PostListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public PostType PostType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public int RepliesCount { get; set; } = -888;
        public int ViewCount { get; set; } = -999;
    }
}