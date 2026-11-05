using DiscussionBoard.Services;
using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Models
{
    /// <summary>
    /// Represents a registered user of the DiscussionBoard application.
    /// Extends IdentityUser to include profile information and relationships to
    /// discussion threads and posts
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the collection od discussion threads
        /// created by the user
        /// </summary>
        public ICollection<DiscussionThread>  DiscussionThreads { get; set; } = new List<DiscussionThread>();

        /// <summary>
        /// Gets or sets the collection of posts and replies created by the user.
        /// </summary>
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        /// <summary>
        /// Gets or sets the display name shown alongside the users discussions threads and posts.
        /// Defaults to an empty string for users who currently do not have a displayed name configured.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier od the users selected avatar. Defaults to the avatar that is defined within
        /// AvatarCatalog
        /// </summary>
        public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
    }
}
