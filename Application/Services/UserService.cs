using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Application.Services;

public sealed class UserService : IUserService
{
private readonly ILogger<UserService> _logger;
private readonly AppDbContext _context;

/// <summary>
/// The Constructor for the UserService class.
/// </summary>
public UserService(ILogger<UserService> logger, AppDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a list of all registered users in the system.
    /// </summary>
    public async Task<List<User>> GetAllUsers()
    {
        _logger.LogDebug("Retrieving all users from the database.");
        var users = await _context.Users.AsNoTracking().ToListAsync();
        _logger.LogDebug("Retrieved {UserCount} users from the database.", users.Count);
        return users;
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    public async Task<User> GetUserByEmail(string email)
    {
        _logger.LogDebug("Retrieving user with email: {Email}", email);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
           _logger.LogError("No user found with email: {Email}", email);
            throw new InvalidOperationException("Invalid email or password.");
        }        
        return user;
    }

    /// <summary>
    /// Authenticates a user with the provided email and password.
    /// </summary>
    public async Task<User> Login(string email, string password)
    {
        _logger.LogDebug("Attempting to log in user with email: {Email}", email);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            _logger.LogError("Login failed: Email or password is null or empty.");
            throw new ArgumentException("Email and password cannot be null or empty.");
        }
        var user = await GetUserByEmail(email);
        if (user == null)
        {
            _logger.LogError("Login failed: No user found with email {Email}.", email);
            throw new InvalidOperationException("Invalid email or password.");
        }
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogError("Login failed: Incorrect password for email {Email}.", email);
            throw new InvalidOperationException("Invalid email or password.");
        }
        return user;

    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    public async Task<User> Register(User? user)
    {
        if (user == null)
        {
            _logger.LogError("Registration failed: User object is null.");
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        // Check if the email exist
        _logger.LogDebug("Attempting to register user with email: {Email}", user.Email);
        var existUser = await _context.Users
                            .AsNoTracking()
                            .AnyAsync(u  => u.Email == user.Email);

        if(existUser)
        {
            _logger.LogError("Registration failed: Email {Email} already exists.", user.Email);
            throw new InvalidOperationException("Email is already registered.");
        }

        //Hash the password before saving
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        //Add the new user into the system
        _context.Users.Add(user);

        //Sync the database
        await _context.SaveChangesAsync();
        
        return user;

    }
}