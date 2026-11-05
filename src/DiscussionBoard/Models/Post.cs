namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents a post or reply within a discussion thread. Inherits common properties from
    /// DiscussionBase and supports nested replies through parent-child relationships.
    /// </summary>
    public class Post : DiscussionBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the discussion thread to which this
        /// post belongs.
        /// </summary>
        public int DiscussionThreadId { get; set; }

        /// <summary>
        /// Gets or sets the discussion thread associated with this post.
        /// </summary>
        public DiscussionThread DiscussionThread { get; set; } = null!;

        /// <summary>
        /// Gets or sets the identifier if the parent post. A null value indicates that this post is a top-level
        /// reply to the discussion thread.
        /// </summary>
        public int? ParentPostId { get; set; }

        /// <summary>
        /// Gets or sets the parent post when this post is a reply to another post.
        /// </summary>
        public Post? ParentPost { get; set; }

        /// <summary>
        /// Gets or sets the collection of direct replies associated with this post.
        /// </summary>
        public ICollection<Post> Replies { get; set; } = new List<Post>();
    }
}
