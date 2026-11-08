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
    /// <summary>
    /// Handles creating a new discussion thread within the forums
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly DiscussionThreadService _threadService;

        private readonly ILogger<CreateModel> _logger;

        /// <summary>
        /// Initialize a new instance of CreateModel
        /// </summary>
        /// <param name="context">
        ///The database context used to access forum data
        /// </param>
        /// <param name="threadService">
        ///The service used to create discussion threads
        /// </param>
        /// <param name="logger">
        ///The logger used to record the application.
        /// </param>
        public CreateModel(ApplicationDbContext context, DiscussionThreadService threadService, ILogger<CreateModel> logger)
        {
            _context = context;
            _threadService = threadService;
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the input data used to create a discussion thread.
        /// </summary>
        [BindProperty]
        public ThreadInput Input { get; set; } = new();

        /// <summary>
        /// Represents the validated input required to create a discussion thread.
        /// </summary>
        public class ThreadInput
        {
            [Required]
            [StringLength(100)]
            public string Title { get; set; } = string.Empty;

            [Required]
            [StringLength(500)]
            public string Content { get; set; } = string.Empty;
        }

        /// <summary>
        /// Displays the discussion creation page after verifying that the forum exists.
        /// </summary>
        /// <param name="forumId">
        ///The ID of the forum where the discuusion will be created.
        /// </param>
        /// <returns>
        ///The creation page if the forum exists else return a 404 response
        /// </returns>
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

        /// <summary>
        /// Validates the submitted input and creates a new discussion thread associated with the specified forum
        /// and current user.
        /// </summary>
        /// <param name="forumId">
        ///The ID of the forum recieving the new discussion thread
        /// </param>
        /// <returns>
        ///A redirect to the forum details page when succesful.
        /// </returns>
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
