using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents the base model for discussion threads and posts.
    /// Contains shared information such as the title, content, creation date, and the author.
    /// </summary>
    public class DiscussionBase : EntityBase
    {
        /// <summary>
        /// Gets or sets the title of the discussion or post. The title is required and cannot exceed
        /// more than 100 characters.
        /// </summary>
        [Required]
        [MaxLength (100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Content of the discussion or post.
        /// The content is required and cannot exceed more than 500 characters
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the discussion or post was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created the discussion or post.
        /// </summary>
        public string ApplicationUserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user associated with this discussion or post
        /// </summary>
        [Required]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
