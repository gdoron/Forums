namespace Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool IsRecipientDeleted { get; set; } 
        public bool IsSenderDeleted { get; set; }

        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string RecipientId { get; set; }
        public ApplicationUser Recipient { get; set; }
    }
}