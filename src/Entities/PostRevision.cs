using System;

namespace Entities
{
    public class PostRevision
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string ChangingUserId { get; set; }
        public ApplicationUser ChangingUser { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }
    }
}