using System.Security.Claims;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace OJudge.Services;

public sealed record JwtSettings(string Issuer, string Audience, SymmetricSecurityKey Key, TimeSpan Lifetime);

public static class JwtAuthentication
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        return int.TryParse(principal.FindFirst("sub")?.Value, NumberStyles.None,
            CultureInfo.InvariantCulture, out var id) && id > 0 ? id : null;
    }

    public static void AddOnlineJudgeAuthentication(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Jwt");
        var secret = section["SigningKey"];
        byte[] key;
        if (string.IsNullOrWhiteSpace(secret) && builder.Environment.IsDevelopment())
        {
            key = RandomNumberGenerator.GetBytes(32);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Set Jwt__SigningKey to a Base64-encoded random key (at least 32 bytes).");
            key = Convert.FromBase64String(secret);
            if (key.Length < 32)
                throw new InvalidOperationException("Jwt__SigningKey must contain at least 32 bytes after Base64 decoding.");
        }

        var settings = new JwtSettings(
            section["Issuer"] ?? "OnlineJudge", section["Audience"] ?? "OnlineJudge.Api",
            new SymmetricSecurityKey(key), TimeSpan.FromMinutes(30));
        builder.Services.AddSingleton(settings);
        builder.Services.AddScoped<JwtSessionService>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, IssuerSigningKey = settings.Key,
                    ValidateIssuer = true, ValidIssuer = settings.Issuer,
                    ValidateAudience = true, ValidAudience = settings.Audience,
                    ValidateLifetime = true, RequireExpirationTime = true, RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero, ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidTypes = ["at+jwt"], NameClaimType = "name", RoleClaimType = "role"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sessions = context.HttpContext.RequestServices.GetRequiredService<JwtSessionService>();
                        if (context.Principal is null ||
                            !await sessions.IsActiveAsync(context.Principal, context.HttpContext.RequestAborted))
                            context.Fail("Session expired or revoked.");
                    }
                };
            });
        builder.Services.AddAuthorization();
    }
}
