using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages
{
    /// <summary>
    /// Handles displaying a forum and its discussion thread
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialize a new instance of the DetailsModel clss
        /// </summary>
        /// <param name="context">
        ///The database context used to retrieve forum data.
        /// </param>
        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets or sets the forum being displayed
        /// </summary>
        public Forum Forum { get; set; } = null!;

        /// <summary>
        /// Gets or sets the discussion threads belonging to a forum
        /// </summary>
        public List<DiscussionThread> Threads { get; set; } = [];

        /// <summary>
        /// Retrieves the requested forum and its discussion threads. Returns a not found response
        /// if the forum does not exist or it has been deleted.
        /// </summary>
        /// <param name="id">
        ///The ID of the forum to retrieve.
        /// </param>
        /// <returns>
        ///The forums details page if found else a 404 response.
        /// </returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var forum = await _context.Forums
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

            if (forum == null)
            {
                return NotFound();
            }

            Forum = forum;

            Threads = await _context.DiscussionThreads
                .Include(d=> d.ApplicationUser)
                .Where(d => d.ForumId == forum.Id)
                .ToListAsync();

            return Page();
        }
    }
}
