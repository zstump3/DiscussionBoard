using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace DiscussionBoard.Pages.Discussions
{
    public class DetailsModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly ILogger<DetailsModel> _logger;
        private readonly PostService _postService;

        public DetailsModel(DiscussionThreadService threadService, ILogger<DetailsModel> logger, PostService postService)
        {
            _threadService = threadService;
            _logger = logger;
            _postService = postService;
        }

        public DiscussionThread Discussion { get; set; } = null!;

        public List<PostTreeBuilder.Node> Posts { get; set; } = [];

        [BindProperty]
        public ReplyInput Input { get; set; } = new();

        public class ReplyInput
        {
            [Required]
            [StringLength(500)]
            public string Content { get; set; } = string.Empty;

            public int? ParentPostId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int threadId)
        {
            var discussion = await _threadService.GetAsync(threadId);

            if (discussion == null)
            {
                _logger.LogWarning("Discussion {ThreadId} was not found.", threadId);

                return NotFound();
            }

            Discussion = discussion;

            var posts = await _postService.GetForThreadAsync(threadId);

            Posts = PostTreeBuilder.Build(posts);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int threadId)
        {
            var discussions = await _threadService.GetAsync(threadId);

            if (discussions == null)
            {
                _logger.LogWarning("Discussion {ThreadId} was not found.", threadId);
                return NotFound();
            }

            Discussion = discussions;

            var posts = await _postService.GetForThreadAsync(threadId);

            if (Input.ParentPostId.HasValue && !posts.Any(p => p.Id == Input.ParentPostId.Value))
            {
                ModelState.AddModelError("Input.ParentPostId", "Invalid parent post.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Reply validation failed for {ThreadId}.", threadId);

                Posts = PostTreeBuilder.Build(posts);

                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var post = new Post
            {
                Title = "Reply",
                Content = Input.Content,
                DiscussionThreadId = threadId,
                ParentPostId = Input.ParentPostId,
                ApplicationUserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _postService.AddAsync(post);

            _logger.LogInformation("Post {PostId} created inside the discussion {ThreadId}.", post.Id, threadId);

            return RedirectToPage("/Discussions/Details", new { threadId });
        }
    }
}
