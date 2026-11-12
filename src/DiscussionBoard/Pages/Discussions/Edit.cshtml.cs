using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    /// <summary>
    /// Handles displaying and updating an existing discussion thread
    /// </summary>
    [Authorize]
    public class ThreadEditModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly ILogger<ThreadEditModel> _logger;
        private readonly IApplicationAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="threadService">
        /// The service used to retrieve and update discussion threads
        /// </param>
        /// <param name="logger">
        /// The logger used to record application events
        /// </param>
        /// <param name="applicationAuthorizationService">
        ///Authorization service to ensure users are authorized
        /// </param>
        public ThreadEditModel(DiscussionThreadService threadService, ILogger<ThreadEditModel> logger,
             IApplicationAuthorizationService applicationAuthorizationService)
        {
            _threadService = threadService;
            _logger = logger;
            _authorizationService = applicationAuthorizationService;
        }

        /// <summary>
        /// Gets or sets the input data used to edit a discussion thread.
        /// </summary>
        [BindProperty]
        public DiscussionEditViewModel Input { get; set; } = new();

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

            if (!await _authorizationService.CanEditAsync(User, thread))
            {
                return Forbid();
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
            var thread = await _threadService.GetAsync(id);

            if (thread == null)
            {
                _logger.LogWarning("Thread with id {ThreadId} not found", id);
                return NotFound();
            }

            if (!await _authorizationService.CanEditAsync(User, thread))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
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
