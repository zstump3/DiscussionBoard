using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services;

/// <summary>
/// Provides database operations for the discussion threads. Handles retrieving, creating,
/// and updating threads within the DiscussionBoard application.
/// </summary>
public class DiscussionThreadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiscussionThreadService> _logger;

    /// <summary>
    /// Initialize the discussion thread service with a database context and logger.
    /// </summary>
    /// <param name="context">
    ///The database context used to access discussion threads.
    /// </param>
    /// <param name="logger">
    ///The logger used to record database errors.
    /// </param>
    public DiscussionThreadService(
        ApplicationDbContext context,
        ILogger<DiscussionThreadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the discussion thread service using a null logger when no logger is provided.
    /// </summary>
    /// <param name="context">
    /// The database context used to access discussion threads.
    /// </param>
    public DiscussionThreadService(ApplicationDbContext context)
        : this(context, NullLogger<DiscussionThreadService>.Instance)
    {
    }

    /// <summary>
    /// Retrieves a discussion thread by its unique identifier. Includes the author and
    /// excludes deleted threads.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the discussion thread.
    /// </param>
    /// <returns>
    /// The matching discussion thread, or null if not found.
    /// </returns>
    public async Task<DiscussionThread?> GetAsync(int id)
    {
        return await _context.DiscussionThreads
            .Include(t => t.ApplicationUser)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    /// <summary>
    /// Retrieves all non-deleted discussion threads belonging to a specified forum.
    /// </summary>
    /// <param name="forumId">
    /// The unique identifier of the forum.
    /// </param>
    /// <returns>
    /// A list of discussion threads belonging to the forum.
    /// </returns>
    public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
    {
        return await _context.DiscussionThreads
            .Where(t => t.ForumId == forumId && !t.IsDeleted)
            .ToListAsync();
    }


    /// <summary>
    /// Adds a new discussion thread to the database. Logs database update errors before rethrowing them.
    /// </summary>
    /// <param name="thread">
    /// The discussion thread to add.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task AddAsync(DiscussionThread thread)
    {
        try
        {
            _context.DiscussionThreads.Add(thread);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error adding the discussion thread.");
            throw;
        }
    }


    /// <summary>
    /// Updates the title and content of an existing discussion thread.
    /// </summary>
    /// <param name="thread">
    /// The discussion thread containing the updated values.
    /// </param>
    /// <returns>
    /// True if the thread was updated successfully
    /// false if the thread could not be found.
    /// </returns>
    public async Task<bool> UpdateAsync(DiscussionThread thread)
    {
        var existing = await GetAsync(thread.Id);

        if (existing == null)
            return false;

        existing.Title = thread.Title;
        existing.Content = thread.Content;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error updating the discussion thread.");
            throw;
        }
    }
}
