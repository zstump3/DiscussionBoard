using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class DiscussionBase : EntityBase
    {
        [Required]
        [StringLength (100)]
        public string Title { get; set; } = String.Empty;

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = String.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string ApplicationId { get; set; } = String.Empty;
        [Required]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
