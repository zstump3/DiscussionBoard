using System.Net.Mime;
using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscussionBoard.Pages.Admin
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationAuthorizationService _authorizationService;

        public List<SelectListItem> UserOptions { get; set; } = new();

        [BindProperty]
        public string? SelectUserId { get; set; }

        public ApplicationUser? SelectUser { get; set; }

        public bool IsSelectedUserAdmin { get; set; }

        public string? StatusMessage { get; set; }

        [BindProperty]
        public string? RoleAction { get; set; }

        public UsersModel(UserManager<ApplicationUser> userManager,  IApplicationAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task OnGetAsync()
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

        public async Task<IActionResult> OnPostAsync()
        {
            await OnGetAsync();

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

        public async Task<IActionResult> OnPostUpdateRoleAsync()
        {
            await OnGetAsync();

            if (string.IsNullOrWhiteSpace(SelectUserId))
            {
                StatusMessage= "Please select a valid user.";
                return Page();
            }

            if (RoleAction != "Add" && RoleAction != "Remove")
            {
                StatusMessage = "Invalid role action.";
                return Page();
            }

            SelectUser = await _userManager.FindByIdAsync(SelectUserId);

            if (SelectUser == null)
            {
                StatusMessage = "There is no such user.";
                return Page();
            }

            IsSelectedUserAdmin = await _userManager.IsInRoleAsync(
                SelectUser, ApplicationRoles.Admin);

            IdentityResult result;

            if (RoleAction == "Remove")
            {
                result = await _authorizationService.RemoveRoleAsync(
                    User, SelectUserId, ApplicationRoles.Admin);
            }
            else
            {
                result = await _authorizationService.AddRoleAsync(
                    User, SelectUserId, ApplicationRoles.Admin);
            }

            if (result.Succeeded)
            {
                StatusMessage = "Admin role has been updated";
            }
            else
            {
                StatusMessage = string.Join(
                    ", ", result.Errors.Select(e => e.Description));
            }

            IsSelectedUserAdmin = await _userManager.IsInRoleAsync(
                SelectUser, ApplicationRoles.Admin);

            return Page();
        }
    }
}
