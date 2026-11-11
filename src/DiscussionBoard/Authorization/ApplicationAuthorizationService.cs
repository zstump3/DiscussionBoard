using System.Security.Claims;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Authorization
{
    public class ApplicationAuthorizationService : IApplicationAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ApplicationAuthorizationService(UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<bool> CanEditAsync(ClaimsPrincipal actingUser, DiscussionBase resource)
        {
            var result = await _authorizationService.AuthorizeAsync(
                actingUser,
                resource,
                ApplicationAuthorizationPolicies.IsOwnerOrAdmin);

            return result.Succeeded;
        }

        public async Task<IdentityResult> AddRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            if (actingUser.Identity?.IsAuthenticated != true ||
                !actingUser.IsInRole(ApplicationRoles.Admin))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "You are not authorized to edit this resource." });
            }

            var actingUserId = _userManager.GetUserId(actingUser);

            var adminUser = await _userManager.FindByIdAsync(actingUserId);

            if (adminUser == null ||
                !await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not an Authorized Administrator." });
            }

            if (roleName != ApplicationRoles.Admin ||
                string.IsNullOrWhiteSpace(targetUserId))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Not a valid user ID or role." });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "User not found." });
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            if (actingUser.Identity?.IsAuthenticated != true ||
                !actingUser.IsInRole(ApplicationRoles.Admin))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not Authorized." });
            }
            var actingUserId = _userManager.GetUserId(actingUser);

            var adminUser = await _userManager.FindByIdAsync(actingUserId);

            if (adminUser == null ||
                !await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not an Authorized Administrator." });
            }

            if (roleName != ApplicationRoles.Admin ||
                string.IsNullOrWhiteSpace(targetUserId))
            {
                return IdentityResult.Failed(
                    new IdentityError{Description = "Not a valid user ID or role." });
            }

            if (actingUserId == targetUserId)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "You cannot remove your own Admin Role." });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);

            if (user == null)
            {
                return IdentityResult.Failed(
                    new IdentityError{Description = "User not found."});
            }

            var admins = await _userManager.GetUsersInRoleAsync(ApplicationRoles.Admin);

            if (admins.Count <= 1)
            {
                return IdentityResult.Failed(
                    new IdentityError{Description = "Cannot remove the last Admin Role."});
            }

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }
    }
}
