using Microsoft.EntityFrameworkCore;
using Serilog;
using Shortly.Application.Interfaces;
using Shortly.Application.Services;
using Shortly.Infrastructure.Persistence;

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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var log = logFactory.CreateLogger<Program>();
    
    log.LogDebug("Initializing..");
    // Aquí irían las migraciones y el seeding
}

app.Run();