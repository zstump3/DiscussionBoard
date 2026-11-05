using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    /// <summary>
    /// Provides user profile validation services for the DiscussionBoard application.
    /// Checks whether display names meet the required length and are available for use.
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UserProfileService class.
        /// </summary>
        /// <param name="context">
        /// The database context used to access registered users.
        /// </param>
        public UserProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Determines whether a display name is valid and available. Trims whitespace, verifies the name length
        /// is between 2 and 60 characters, and performs a case-insensitive database lookup for
        /// duplicate display names.
        /// </summary>
        /// <param name="displayName">
        /// The display name to validate and check for availability.
        /// </param>
        /// <param name="excludeUserId">
        /// The optional user ID to exclude from the duplicate check.
        /// Allows existing users to keep their current display name.
        /// </param>
        /// <returns>
        /// True if the display name is valid and available;
        /// otherwise, false.
        /// </returns>
        public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
        {
            //Reject null, empty, or whitespace display names.
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            displayName = displayName.Trim();

            //Ensure the display name meets the required length.
            if (displayName.Length < 2 || displayName.Length > 60)
            {
                return false;
            }

            var normalizeName = displayName.ToUpperInvariant();

            var query = _context.Users.AsQueryable();

            //Exclude the current user when updating their profile.
            if (excludeUserId != null)
            {
                query = query.Where(user => user.Id != excludeUserId);
            }

            //Check wether another user already has this display name.
            var nameExists = await query.AnyAsync(
                user => user.DisplayName.ToUpper() == normalizeName);

            return !nameExists;
        }
    }
}
