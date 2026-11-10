using System.Net.Mime;
using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscussionBoard.Pages.Amin
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public List<SelectListItem> UserOptions { get; set; } = new();

        [BindProperty]
        public string? SelectUserId { get; set; }

        [BindProperty]
        public ApplicationUser? SelectUser { get; set; }

        public bool IsSelectedUserAdmin { get; set; }

        public UsersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task onGetAsync()
        {
            UserOptions = _userManager.Users
                .OrderBy(u => u.Email)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.DisplayName} ({u.Email})"
                })
                .ToList();
        }

        public async Task<IActionResult> onPostAsync()
        {
            await onGetAsync();

            if (string.IsNullOrWhiteSpace(SelectUserId))
            {
                ModelState.AddModelError(
                    nameof(SelectUserId),
                    "Please select a user.");
                return Page();
            }

            SelectUser = await _userManager.FindByIdAsync(SelectUserId);

            if (SelectUser == null)
            {
                ModelState.AddModelError(
                    nameof(SelectUserId),
                    "There is no such user.");
                return Page();
            }

            IsSelectedUserAdmin = await _userManager.IsInRoleAsync(
                SelectUser, ApplicationRoles.Admin);

            return Page();
        }
    }
}
