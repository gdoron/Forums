using System.ComponentModel.DataAnnotations;
using Entities;

namespace Forums.Models
{
    public class CreateUserReviewModel
    {
        [Required]
        public string ToUserId { get; set; }
        public string Review { get; set; }
        [Required]
        public VoteType VoteType { get; set; } 
    }
}