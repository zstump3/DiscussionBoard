using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    /// <summary>
    /// Provides database operations for forums within the DiscussionBoard application.
    /// </summary>
    public class ForumService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ForumService class.
        /// </summary>
        /// <param name="context">
        /// The database context used to retrieve forum data.
        /// </param>
        public ForumService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all forums that have not been marked as deleted.
        /// </summary>
        /// <returns>
        /// A task containing a list of non-deleted forums.
        /// Returns an empty list if no forums are found.
        /// </returns>
        public async Task<List<Forum>> GetAllAsync()
        {
            return await _context.Forums
                .Where(f => f.IsDeleted == false)
                .ToListAsync();
        }
    }
}
