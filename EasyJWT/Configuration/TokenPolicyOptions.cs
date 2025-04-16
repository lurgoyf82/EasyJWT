using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Configuration;

/// <summary>Definisce il comportamento di una particolare tipologia di token.</summary>
public sealed class TokenPolicyOptions
{
    public string Name { get; init; } = default!;

    public string SigningAlgorithm { get; set; } = SecurityAlgorithms.EcdsaSha256;

    public bool EncryptToken { get; set; }

    public TimeSpan ExpireTime { get; set; } = TimeSpan.FromMinutes(5);

    public bool IncludeJwtId { get; set; } = true;

    public string TypHeader { get; set; } = "at+jwt";

    public TokenValidationParameters ToValidationParameters(SecurityKey key,
                                                            IEnumerable<string> audiences,
                                                            string issuer)
        => new()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudiences = audiences,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidAlgorithms = new[] { SigningAlgorithm }
        };
}