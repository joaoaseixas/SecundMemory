using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Infrastructure.Identity;

public class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    private const string WorkspaceIdClaimType = "workspace_id";
    private readonly JwtSettings _settings = options.Value;

    public AccessToken GenerateToken(Guid userId, Guid workspaceId, IEnumerable<string> roles)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(WorkspaceIdClaimType, workspaceId.ToString()),
            ..roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims, expires: expiresAtUtc, signingCredentials: credentials);
        var value = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(value, expiresAtUtc);
    }

    public TokenPrincipal? ReadPrincipalIgnoringExpiry(string token)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = false, // refresh flow: signature must still check out, expiry is intentionally not enforced here
        };

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);

            var userId = Guid.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var workspaceId = Guid.Parse(principal.FindFirstValue(WorkspaceIdClaimType)!);
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return new TokenPrincipal(userId, workspaceId, roles);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException or FormatException or NullReferenceException)
        {
            return null;
        }
    }
}
