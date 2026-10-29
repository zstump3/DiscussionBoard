using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Discussions
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly DiscussionThreadService _threadService;

        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, DiscussionThreadService threadService, ILogger<CreateModel> logger)
        {
            _context = context;
            _threadService = threadService;
            _logger = logger;
        }

        [BindProperty]
        public ThreadInput Input { get; set; } = new();

        public class ThreadInput
        {
            [Required]
            [StringLength(100)]
            public string Title { get; set; } = string.Empty;

            [Required]
            [StringLength(500)]
            public string Content { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int forumId)
        {
            bool exists = await _context.Forums
                .AnyAsync(f => f.Id == forumId && !f.IsDeleted);

            if (!exists)
            {
                _logger.LogWarning("Forum with id {forumId} not found", forumId);
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int forumId)
        {
            bool exists = await _context.Forums
                .AnyAsync(f => f.Id == forumId && !f.IsDeleted);

            if (!exists)
            {
                _logger.LogWarning("Forum with id {forumId} not found", forumId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var thread = new DiscussionThread
            {
                Title = Input.Title,
                Content = Input.Content,
                ForumId = forumId,
                ApplicationUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

                await _threadService.AddAsync(thread);

                _logger.LogInformation("Discussion thread with id {forumId} created", forumId);

                return RedirectToPage("/Forums/Details",
                    new {id = forumId});
        }

    }
}
