using System;

namespace Forums.Models
{
    public class OutgoingMessageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime SentDate { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUserName { get; set; }
    }
}