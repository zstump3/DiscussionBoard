using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents a forum within the DiscussionBoard application. Contains the
    /// forums name, description, and collection of associated discussion threads.
    /// </summary>
    public class Forum : EntityBase
    {
        /// <summary>
        /// Gets or sets the name of the forum. The name is required and cannot exceed more than
        /// 100 characters.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the forum. The description cannot exceed more than
        /// 500 characters.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        //public DiscussionThread? DiscussionThreadId { get; set; }

        /// <summary>
        /// Gets or sets the collection of discussion threads belonging to this forum.
        /// </summary>
        public ICollection<DiscussionThread> DiscussionThreads  { get; set; } = new List<DiscussionThread>();
    }
}
