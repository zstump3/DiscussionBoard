using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services;

/// <summary>
/// Provides database operations for posts and replies within discussion threads.
/// Handles retrieving and creating posts using Entity Framework Core.
/// </summary>
public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostService> _logger;

    /// <summary>
    /// Initializes a new instance of the PostService class with a database context and logger.
    /// </summary>
    /// <param name="context">
    /// The database context used to access posts.
    /// </param>
    /// <param name="logger">
    /// The logger used to record database errors.
    /// </param>
    public PostService(
        ApplicationDbContext context,
        ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public PostService(ApplicationDbContext context)
        : this(context, NullLogger<PostService>.Instance)
    {
    }

    /// <summary>
    /// Retrieves all non-deleted posts and replies belonging to a specified discussion thread.
    /// Includes the author associated with each post.
    /// </summary>
    /// <param name="threadId">
    /// The unique identifier of the discussion thread.
    /// </param>
    /// <returns>
    /// A task containing a list of posts and replies.
    /// Returns an empty list if no posts are found.
    /// </returns>
    public async Task<List<Post>> GetForThreadAsync(int threadId)
    {
        return await _context.Posts
            .Include(p => p.ApplicationUser)
            .Where(p => p.DiscussionThreadId == threadId && !p.IsDeleted)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new post or reply to the database. Logs database update errors before rethrowing them.
    /// </summary>
    /// <param name="post">
    /// The post or reply to add to the database.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task AddAsync(Post post)
    {
        try
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error adding post.");
            throw;
        }
    }
}
