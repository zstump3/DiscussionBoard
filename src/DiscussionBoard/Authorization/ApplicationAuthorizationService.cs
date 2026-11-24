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
        private readonly ILogger<ApplicationAuthorizationService> _logger;

        public ApplicationAuthorizationService(UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService, ILogger<ApplicationAuthorizationService> logger)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _logger = logger;
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
                _logger.LogWarning(
                    "Unauthorize attempt to grant administrator role.");

                return IdentityResult.Failed(
                    new IdentityError { Description = "You are not authorized to edit this resource." });
            }

            var actingUserId = _userManager.GetUserId(actingUser);

            var adminUser = await _userManager.FindByIdAsync(actingUserId);

            if (adminUser == null ||
                !await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
            {
                _logger.LogWarning(
                    "Administrator verification failed for user {ActingUserId} attempting to add role.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not an Authorized Administrator." });
            }

            if (roleName != ApplicationRoles.Admin ||
                string.IsNullOrWhiteSpace(targetUserId))
            {
                _logger.LogWarning(
                    "Role denied for administrator {ActingUserId} due to an invalid user ID or role.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError { Description = "Not a valid user ID or role." });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);

            if (user == null)
            {
                _logger.LogWarning(
                    "Role addition failed for administrator {ActingUserId} because the user was not found.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError { Description = "User not found." });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Role {RoleName} added to user {TargetUserId} by admin {ActingUserId}",
                    roleName,
                    targetUserId,
                    actingUserId);
            }
            else
            {
                _logger.LogWarning(
                    "Role {RoleName} failed for user {TargetUserId} by admin {ActingUserId}",
                    roleName,
                    targetUserId,
                    actingUserId);
            }
            return result;
        }

        public async Task<IdentityResult> RemoveRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            if (actingUser.Identity?.IsAuthenticated != true ||
                !actingUser.IsInRole(ApplicationRoles.Admin))
            {
                _logger.LogWarning(
                    "Unauthorize attempt to remove administrator role.");

                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not Authorized." });
            }
            var actingUserId = _userManager.GetUserId(actingUser);

            var adminUser = await _userManager.FindByIdAsync(actingUserId);

            if (adminUser == null ||
                !await _userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
            {
                _logger.LogWarning(
                    "Administrator verification failed for user {ActingUserId} attempting to remove role.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError { Description = "User is not an Authorized Administrator." });
            }

            if (roleName != ApplicationRoles.Admin ||
                string.IsNullOrWhiteSpace(targetUserId))
            {
                _logger.LogWarning(
                    "Role removal denied for administrator {ActingUserId} due to an invalid user ID or role.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError{Description = "Not a valid user ID or role." });
            }

            if (actingUserId == targetUserId)
            {
                _logger.LogWarning(
                    "Administrator {ActingUserId} attempted to remove their own role {RoleName}",
                    actingUserId,
                    roleName);

                return IdentityResult.Failed(
                    new IdentityError { Description = "You cannot remove your own Admin Role." });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);

            if (user == null)
            {
                _logger.LogWarning(
                    "Role removal failed for administrator {ActingUserId} because the user was not found.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError{Description = "User not found."});
            }

            var admins = await _userManager.GetUsersInRoleAsync(ApplicationRoles.Admin);

            if (admins.Count <= 1)
            {
                _logger.LogWarning(
                    "Administrator {ActingUserId} attempted to remove the last administrator role.",
                    actingUserId);

                return IdentityResult.Failed(
                    new IdentityError{Description = "Cannot remove the last Admin Role."});
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Role {RoleName} removed from the user {TargetUserId} by admin {ActingUserId}",
                    roleName,
                    targetUserId,
                    actingUserId);
            }
            else
            {
                _logger.LogWarning(
                    "Role {RoleName} could not be removed from the user {TargetUserId} by admin {ActingUserId}",
                    roleName,
                    targetUserId,
                    actingUserId);
            }
            return result;
        }
    }
}
