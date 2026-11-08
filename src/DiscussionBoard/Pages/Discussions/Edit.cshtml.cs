using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    /// <summary>
    /// Handles displaying and updating an existing discussion thread
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly ILogger<EditModel> _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="threadService">
        /// The service used to retrieve and update discussion threads
        /// </param>
        /// <param name="logger">
        /// The logger used to record application events
        /// </param>
        public EditModel(DiscussionThreadService threadService, ILogger<EditModel> logger)
        {
            _threadService = threadService;
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the input data used to edit a discussion thread.
        /// </summary>
        [BindProperty]
        public ThreadInput Input { get; set; } = new();

        /// <summary>
        /// Represents the validated input required to update a discussion thread.
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
        /// Retrieves the specified discussion thread and populates the edit form
        /// with its existing title and content.
        /// </summary>
        /// <param name="id">
        /// The ID of the discussion thread to edit
        /// </param>
        /// <returns>
        /// The edit page if the thread exists else a 404 response.
        /// </returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var thread = await _threadService.GetAsync(id);

            if (thread == null)
            {
                _logger.LogWarning("Thread with id {ThreadId} not found", id);
                return NotFound();
            }

            Input.Title = thread.Title;
            Input.Content = thread.Content;

            return Page();
        }

        /// <summary>
        /// Validates the submitted changes and updates the specified discussion thread.
        /// </summary>
        /// <param name="id">
        /// The ID of the discussion thread to update
        /// </param>
        /// <returns>
        /// A redirect to the forum details page when successful or a 404 response
        /// if the thread cannot be found or updated.
        /// </returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Thread with id {ThreadId} not found", id);
                return Page();
            }
            var thread = await _threadService.GetAsync(id);

            if (thread == null)
            {
                _logger.LogWarning("Thread with id {ThreadId} not found", id);
                return NotFound();
            }

            var updatedThread = new DiscussionThread
            {
                Id = thread.Id,
                Title = Input.Title,
                Content = Input.Content,
            };

            bool updated = await _threadService.UpdateAsync(updatedThread);

            if (!updated)
            {
                _logger.LogWarning("Thread with id {ThreadId} could not be updated.", id);

                return NotFound();
            }
            _logger.LogInformation("Thread with id {ThreadId} was updated", id);

            return RedirectToPage("/Forums/Details", new { id = thread.ForumId });
        }
    }
}
