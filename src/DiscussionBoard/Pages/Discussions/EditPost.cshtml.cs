using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class EditPostModel : PageModel
    {
        private readonly PostService _postService;
        private readonly IApplicationAuthorizationService _authorizationService;

        public EditPostModel(
            PostService postService,
            IApplicationAuthorizationService authorizationService)
            {
            _postService = postService;
            _authorizationService = authorizationService;
            }

        [BindProperty] public DiscussionEditViewModel Input { get; set; } = new();

        /// <summary>
        /// Retrieves a post and displays its existing title and content, only if the
        /// current user is authorized to edit it.
        /// </summary>
        /// <param name="threadId">The discussion thread ID</param>
        /// <param name="postId">The post ID</param>
        /// <returns>The edit page, NotFound, or Forbid</returns>
        public async Task<IActionResult> OnGetAsync(int threadId, int postId)
        {
            var post = await _postService.GetAsync(postId);

            if (post == null ||
                post.Id != postId ||
                post.DiscussionThreadId != threadId)
            {
                return NotFound();
            }

            if (!await _authorizationService.CanEditAsync(User, post))
            {
                return Forbid();
            }

            Input.Title = post.Title;
            Input.Content  = post.Content;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int threadId, int postId)
        {
            var post = await _postService.GetAsync(postId);

            if (post == null ||
                post.Id != postId ||
                post.DiscussionThreadId != threadId)
            {
                return NotFound();
            }

            if (!await _authorizationService.CanEditAsync(User, post))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedPost = new Post
            {
                Id = postId,
                Title = Input.Title,
                Content = Input.Content
            };

            bool updated = await _postService.UpdateAsync(updatedPost);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToPage("/Discussions/Details",
                new {threadId});
        }
    }
}
