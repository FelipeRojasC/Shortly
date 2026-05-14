using Shortly.Domain.Entities;
namespace Shortly.Application.Interfaces;


public interface ILinkService
{
    /// <summary>
    /// Creates a new shortened link.
    /// </summary>
    Task<Link> CreateLink(Link link);

    /// <summary>
    /// Increments the click count for a given short URL and user ID.
    /// </summary>
    Task<Link> IncrementClicks(string url, long userId);

    /// <summary>
    /// Retrieves a link by its short URL.
    /// </summary>
    Task<Link> GetLink(string shortUrl);

    /// <summary>
    /// Retrieves all links in the system.
    /// </summary> 
    Task<List<Link>> GetAllLinks();
}