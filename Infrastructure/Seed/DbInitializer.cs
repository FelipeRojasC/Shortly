using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.Seed;

public class DbInitializer
{
    // Cambiado a 'Seed' y 'Task' según el diagrama 
    public async Task Seed(AppDbContext context, ILogger<DbInitializer> log)
    {
        // Aplica migraciones automáticamente al iniciar 
        log.LogInformation("Applying migrations...");
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync()) return;

        log.LogInformation("Seeding initial data...");

        // 1. Crear usuarios
        var user1 = new User("carl.johnson@test.com", "password321");
        var user2 = new User("will.smith@test.com", "password654");

        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();
        log.LogInformation("Database seeded successfully.");

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