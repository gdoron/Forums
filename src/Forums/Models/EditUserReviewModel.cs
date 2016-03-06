using System.ComponentModel.DataAnnotations;
using Entities;

namespace Forums.Models
{
    public class EditUserReviewModel
    {
        [Required]
        public int Id { get; set; }
        public string Review { get; set; }
        [Required]
        public VoteType VoteType { get; set; }
    }
}