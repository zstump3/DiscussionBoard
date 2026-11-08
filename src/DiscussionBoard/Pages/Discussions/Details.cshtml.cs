using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace DiscussionBoard.Pages.Discussions
{
    /// <summary>
    /// Handles displaying a discussion thread and its nested replies.
    /// Allows authenticated users to submit replies to discussions or existing posts.
    /// </summary>
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly DiscussionThreadService _threadService;
        private readonly ILogger<DetailsModel> _logger;
        private readonly PostService _postService;

        /// <summary>
        /// Initializes a new instance of the DetailsModel class.
        /// </summary>
        /// <param name="threadService">
        /// The service used to retrieve discussion threads.
        /// </param>
        /// <param name="logger">
        /// The logger used to record application events.
        /// </param>
        /// <param name="postService">
        /// The service used to retrieve and create posts.
        /// </param>
        public DetailsModel(DiscussionThreadService threadService, ILogger<DetailsModel> logger, PostService postService)
        {
            _threadService = threadService;
            _logger = logger;
            _postService = postService;
        }

        /// <summary>
        /// Gets or sets the discussion thread being displayed
        /// </summary>
        public DiscussionThread Discussion { get; set; } = null!;

        /// <summary>
        /// Gets or sets the input data submitted when created a reply
        /// </summary>
        public List<PostTreeBuilder.Node> Posts { get; set; } = [];

        /// <summary>
        /// Gets or sets the input data submitted when creating a reply.
        /// </summary>
        [BindProperty]
        public ReplyInput Input { get; set; } = new();

        /// <summary>
        /// Represents the validated input required to create a discussion reply.
        /// </summary>
        public class ReplyInput
        {
            [Required]
            [StringLength(500)]
            public string Content { get; set; } = string.Empty;

            public int? ParentPostId { get; set; }
        }

        /// <summary>
        /// Retrieves the requested discussion thread and builds its nested post hierarchy.
        /// </summary>
        /// <param name="threadId">
        /// The ID of the discussion thread to retrieve
        /// </param>
        /// <returns>
        /// The discussion details page if found else a 404 response.
        /// </returns>
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

        /// <summary>
        /// Validates the submitted reply and creates a new post associated
        /// with the current discussion and authenticated user.
        /// </summary>
        /// <param name="threadId">The ID of the discussion thread receiving the reply
        /// </param>
        /// <returns>
        /// A redirect to the discussion details page when successful,
        /// or a 404 response if the discussion thread does not exist
        /// </returns>
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
