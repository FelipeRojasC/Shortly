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

        if (await context.Users.AnyAsync())
        {
            return;
        }

        log.LogInformation("Seeding initial users...");
        var users = new User[]
        {
            new User("carl.johnson@test.com", "password321"),
            new User("will.smith@test.com", "password654")
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        log.LogInformation("Database seeded successfully.");
    }
}