using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewsModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    public class MyProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;

        public MyProfileModel(UserManager<ApplicationUser> userManager, IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
        }

        [BindProperty]
        public UserProfile Input { get; set; } = new();

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
