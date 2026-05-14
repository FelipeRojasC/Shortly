using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Application.Services;

public sealed class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;
    private readonly AppDbContext _context;

    /// <summary>
    /// The Constructor for the LinkService class.
    /// </summary>
    public LinkService(ILogger<LinkService> logger, AppDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Creates a new shortened link in the system.
    /// </summary>
    public async Task<Link> CreateLink(Link link)
    {
        if (link == null)
        {
            _logger.LogError("Link creation failed: Link object is null.");
            throw new ArgumentNullException(nameof(link), "Link cannot be null.");
        }
        _logger.LogDebug("Attempting to create a new link for Original URL: {OriginalUrl}", link.Url);

        _context.Links.Add(link);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Link successfully created with Short URL: {ShortUrl}", link.ShortUrl);
        return link;
    }

    /// <summary>
    /// Retrieves a list of all links registered in the system.
    /// </summary>
    public async Task<List<Link>> GetAllLinks()
    {
        _logger.LogDebug("Retrieving all links from the database.");
        var links = await _context.Links.AsNoTracking().ToListAsync();
        _logger.LogDebug("Retrieved {LinkCount} links from the database.", links.Count);
        return links;
    }

    /// <summary>
    /// Retrieves a link by its short URL code.
    /// </summary>
    public async Task<Link> GetLink(string shortUrl)
    {
        _logger.LogDebug("Retrieving link with Short URL: {ShortUrl}", shortUrl);

        var link = await _context.Links
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.ShortUrl == shortUrl);

        if (link == null)
        {
            _logger.LogWarning("No link found with Short URL: {ShortUrl}", shortUrl);
            throw new KeyNotFoundException($"The link with code {shortUrl} was not found.");
        }

        return link;
    }

    /// <summary>
    /// Increments the click count for a specific link and user.
    /// </summary>
    public async Task<Link> IncrementClicks(string shortUrl, long userId)
    {
        _logger.LogDebug("Attempting to increment clicks for Short URL: {ShortUrl} by User ID: {UserId}", shortUrl, userId);

        var link = await _context.Links
            .FirstOrDefaultAsync(l => l.ShortUrl == shortUrl && l.UserId == userId);

        if (link == null)
        {
            _logger.LogError("Increment clicks failed: Link {ShortUrl} not found for User ID {UserId}.", shortUrl, userId);
            throw new KeyNotFoundException("No link found for the specified user and URL.");
        }

        link.IncrementClicks(); 
        
        await _context.SaveChangesAsync();

        _logger.LogInformation("Click incremented successfully. New total for {ShortUrl}: {Clicks}", shortUrl, link.Clicks);
        
        return link;
    }
}