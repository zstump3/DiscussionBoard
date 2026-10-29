using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    public class EditModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(DiscussionThreadService threadService, ILogger<EditModel> logger)
        {
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
