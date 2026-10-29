using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscussionBoard.Services;

public class DiscussionThreadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiscussionThreadService> _logger;

    public DiscussionThreadService(
        ApplicationDbContext context,
        ILogger<DiscussionThreadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public DiscussionThreadService(ApplicationDbContext context)
        : this(context, NullLogger<DiscussionThreadService>.Instance)
    {
    }

    public async Task<DiscussionThread?> GetAsync(int id)
    {
        return await _context.DiscussionThreads
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
    {
        return await _context.DiscussionThreads
            .Where(t => t.ForumId == forumId && !t.IsDeleted)
            .ToListAsync();
    }

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
