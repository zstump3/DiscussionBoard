using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Services;

namespace DiscussionBoard.ViewsModel
{
    /// <summary>
    /// Represents the editable profile information for a user. Contains the user's display
    /// name and selected avatar. Used by the registration and profile update forms.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets or sets the user's display name.
        /// The name is required and must contain between
        /// 2 and 60 characters.
        /// </summary>
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the user's selected avatar.
        /// Defaults to the avatar defined by AvatarCatalog.
        /// </summary>
        [Required]
        public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
    }
}
