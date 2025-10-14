using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Zero.Api.Models.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Zero.Api.Services;

public class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int ExpiresInMinutes { get; set; } = 60;
}

public interface IJwtTokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(AppUser user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly UserManager<AppUser> _userManager;

    public JwtTokenService(IOptions<JwtOptions> options, UserManager<AppUser> userManager)
    {
        _options = options.Value;
        _userManager = userManager;
    }

    public async Task<(string Token, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(AppUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("izaroCode", user.IzaroCode)
        };
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }
}