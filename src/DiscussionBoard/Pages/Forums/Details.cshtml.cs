using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Forum Forum { get; set; } = null!;

        public List<DiscussionThread> Threads { get; set; } = [];

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
                .Where(d => d.ForumId == forum.Id)
                .ToListAsync();

            return Page();
        }
    }
}
