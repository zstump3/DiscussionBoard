namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents a discussion thread within a forum, Inherits common discussion properties from
    /// DiscussionBase and maintains relationships with its forum and posts.
    /// </summary>
    public class DiscussionThread : DiscussionBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the forum to which this discussion thread belongs.
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the forum associated with this discussion thread
        /// </summary>
        public Forum Forum { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of posts and replies associated with this discussion thread.
        /// </summary>
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
