using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieHub.API.Data;
using MovieHub.API.DTOs.Auth;
using MovieHub.API.Models;
using MovieHub.API.Options;
using MovieHub.API.Exceptions;

namespace MovieHub.API.Services;

public class AuthService
{
    private readonly AppDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext dbContext, IOptions<JwtSettings> jwtSettings)
    {
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var usernameTaken = await _dbContext.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (usernameTaken)
        {
            throw new DuplicateUserException("Username is already taken.");
        }

        var emailTaken = await _dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailTaken)
        {
            throw new DuplicateUserException("Email is already taken.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User,
            RegisteredAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);


        const string dummyHash = "$2a$12$1mavJkj7uSQ89BItMicdy.zYRsGhk8Ky7/Rytjv22SmnOgMY7pIeq";
        string hashToVerify = user != null ? user.PasswordHash : dummyHash;

        bool passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, hashToVerify);

        if (user is null || !passwordIsValid)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }


        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        return new AuthResponse
        {
            AccessToken = GenerateJwtToken(user),
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(_jwtSettings.Issuer) ? null : _jwtSettings.Issuer,
            audience: string.IsNullOrWhiteSpace(_jwtSettings.Audience) ? null : _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


