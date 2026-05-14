using Microsoft.EntityFrameworkCore;
using Serilog;
using Shortly.Application.Interfaces;
using Shortly.Application.Services;
using Shortly.Infrastructure.Persistence;
using Shortly.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext") ?? 
    throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Host.UseSerilog((hostingContext, services, configuration) => {
    configuration.ReadFrom.Configuration(hostingContext.Configuration);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>(); 
    
    try 
    {
        logger.LogDebug("Initializing Database and Seeding...");
        
        DbInitializer initializer = new DbInitializer();
        initializer.Seed(dbContext, services.GetRequiredService<ILogger<DbInitializer>>())
                     .GetAwaiter()
                     .GetResult();
                     
        logger.LogDebug("Initialization OK.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}

app.Run();