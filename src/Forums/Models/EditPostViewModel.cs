using System.ComponentModel.DataAnnotations;

namespace Forums.Models
{
    public class EditPostViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}