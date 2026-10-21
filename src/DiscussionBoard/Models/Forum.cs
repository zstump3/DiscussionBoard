using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class Forum : EntityBase
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = String.Empty;

        [StringLength(500)]
        public string Description { get; set; } = String.Empty;
    }
}
