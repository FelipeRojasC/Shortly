using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.Seed;

/// <summary>
/// Handles the initial database setup, including migrations and seeding default data.
/// </summary>
public class DbInitializer
{
    /// <summary>
    /// Applies pending migrations and seeds the database with initial users and links if empty.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="log">Logger instance for tracking the seeding process.</param>
    public async Task Seed(AppDbContext context, ILogger<DbInitializer> log)
    {
        // Automatically apply any pending migrations on startup 
        log.LogInformation("Applying migrations...");
        await context.Database.MigrateAsync();

        // Check if the database already contains users to avoid duplicate seeding
        if (await context.Users.AnyAsync()) return;

        log.LogInformation("Seeding initial data...");

        // 1. Create default test users 
        var user1 = new User("carl.johnson@test.com", "password321");
        var user2 = new User("will.smith@test.com", "password654");

        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync(); // Save changes to generate User IDs for the foreign key relationship
        log.LogInformation("Database seeded successfully with users.");

        // 2. Create sample links associated with the seeded users 
        var links = new Link[]
        {
            new Link("https://www.google.com", "goog", user1.Id),
            new Link("https://github.com/FelipeRojasC", "my-git", user1.Id),
            new Link("https://www.ucn.cl", "ucn", user2.Id)
        };

        await context.Links.AddRangeAsync(links);
        await context.SaveChangesAsync();

        log.LogInformation("Database seeded with users and links.");
    }
}