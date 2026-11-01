using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;

        public UserProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            displayName = displayName.Trim();

            if (displayName.Length < 2 || displayName.Length > 60)
            {
                return false;
            }

            var normalizeName = displayName.ToUpperInvariant();

            var query = _context.Users.AsQueryable();

            if (excludeUserId != null)
            {
                query = query.Where(user => user.Id != excludeUserId);
            }

            var nameExists = await query.AnyAsync(
                user => user.DisplayName.ToUpper() == normalizeName);

            return !nameExists;
        }
    }
}
