using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Services;

namespace DiscussionBoard.ViewsModel
{
    public class UserProfile
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
    }
}
