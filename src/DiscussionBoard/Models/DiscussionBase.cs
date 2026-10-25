using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class DiscussionBase : EntityBase
    {
        [Required]
        [StringLength (100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        [Required]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
