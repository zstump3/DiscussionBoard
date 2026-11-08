using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewsModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    /// <summary>
    /// Handles displaying and updating the current user's profile information.
    /// </summary>
    public class MyProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;


        /// <summary>
        /// Initializes a new instance of the MyProfileModel class.
        /// </summary>
        /// <param name="userManager">
        /// The manager used to retrieve and update user accounts
        /// </param>
        /// <param name="userProfileService">
        /// The service used to validate display name availability
        /// </param>
        public MyProfileModel(UserManager<ApplicationUser> userManager, IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Gets or sets the profile information submitted by the user.
        /// </summary>
        [BindProperty]
        public UserProfile Input { get; set; } = new();

        /// <summary>
        /// Retrieves the current user's profile information
        /// and populates the profile form.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return;
            }

            Input = new UserProfile
            {
                DisplayName = user.DisplayName,
                AvatarId = user.AvatarId
            };
        }

        /// <summary>
        /// Validates and saves changes to the current user's display name and selected avatar.
        /// </summary>
        /// <returns>
        /// A redirect to the profile page when successful,
        /// the current page when validation fails, or an authentication
        /// challenge if the user cannot be retrieved.
        /// </returns>
        public async Task<IActionResult> OnPostAsync()
        {
            Input.DisplayName = (Input.DisplayName ?? string.Empty).Trim();

            if (Input.DisplayName.Length < 2 || Input.DisplayName.Length > 60)
            {
                ModelState.AddModelError(
                    "Input.DisplayName",
                    "Display name must be between 2 and 60.");
            }

            if (!AvatarCatalog.IsValid(Input.AvatarId))
            {
                ModelState.AddModelError(
                    "Input.AvatarId",
                    "Please select an avatar.");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            if (Input.DisplayName.Length >= 2 && Input.DisplayName.Length <= 60)
            {
                bool isAvailable = await _userProfileService.IsDisplayNameAvailableAsync(
                    Input.DisplayName,
                    user.Id);

                if (!isAvailable)
                {
                    ModelState.AddModelError(
                        "Input.DisplayName",
                        "This display name is already in use.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.DisplayName = Input.DisplayName;
            user.AvatarId = Input.AvatarId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }
                return Page();
            }

            TempData["SuccessMessage"] = "Your profile has been successfully updated.";
            return RedirectToPage();
        }
    }
}
