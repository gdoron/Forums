using System;

namespace Forums.Models
{
    public class IncomingMessageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
    }
}