using System.Globalization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OJudge.Data;
using OJudge.Dtos;
using OJudge.Models;

namespace OJudge.Services;

public sealed class JwtSessionService(AppDbContext db, JwtSettings settings)
{
    public async Task<TokenDto> IssueAsync(User user, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var session = new JwtSession
        {
            Id = Guid.NewGuid(), UserId = user.Id, ExpiresAt = now.Add(settings.Lifetime)
        };
        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer, Audience = settings.Audience, TokenType = "at+jwt",
            IssuedAt = now, NotBefore = now, Expires = session.ExpiresAt,
            SigningCredentials = new SigningCredentials(settings.Key, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Id.ToString(CultureInfo.InvariantCulture), ["jti"] = session.Id.ToString("D"),
                ["name"] = user.NickName, ["role"] = user.Role.Title
            }
        });
        await db.JwtSessions.Where(s => s.ExpiresAt <= now).ExecuteDeleteAsync(ct);
        db.JwtSessions.Add(session);
        await db.SaveChangesAsync(ct);
        return new TokenDto { AccessToken = token, ExpiresAt = session.ExpiresAt };
    }

    public async Task<bool> IsActiveAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var userId = principal.GetUserId();
        var role = principal.FindFirst("role")?.Value;
        if (userId is null || role is not (AuthRoles.User or AuthRoles.Admin) ||
            !Guid.TryParseExact(principal.FindFirst("jti")?.Value, "D", out var sessionId))
            return false;

        var now = DateTime.UtcNow;
        return await db.JwtSessions.AnyAsync(s => s.Id == sessionId && s.UserId == userId &&
            s.ExpiresAt > now && s.User.Role.Title == role, ct);
    }

    public Task RevokeAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var sessionId = Guid.Parse(principal.FindFirst("jti")!.Value);
        var userId = principal.GetUserId();
        return db.JwtSessions.Where(s => s.Id == sessionId && s.UserId == userId).ExecuteDeleteAsync(ct);
    }
}
