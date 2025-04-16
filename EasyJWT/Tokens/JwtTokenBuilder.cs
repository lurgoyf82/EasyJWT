using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EasyJWT.Configuration;
using EasyJWT.Keys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace EasyJWT.Tokens;

/// <summary>
/// Costruttore fluido di JWT; incapsula le opzioni di policy, tenant e claim.
/// </summary>
public sealed class JwtTokenBuilder
{
    private readonly JwtLibraryOptions _options;
    private readonly IKeyProvider _keyProvider;
    private readonly string _policyName;
    private readonly string? _tenantId;
    private readonly Dictionary<string, object> _claims = new(StringComparer.Ordinal);

    private string? _subject;
    private string? _audienceOverride;
    private DateTimeOffset? _expiresOverride;

    private JwtTokenBuilder(JwtLibraryOptions options,
                            IKeyProvider keyProvider,
                            string policyName,
                            string? tenantId)
    {
        _options = options;
        _keyProvider = keyProvider;
        _policyName = policyName;
        _tenantId = tenantId;
    }

    public static JwtTokenBuilder New(string policyName,
                                      IServiceProvider sp,
                                      string? tenantId = null)
    {
        var opts = sp.GetRequiredService<IOptions<JwtLibraryOptions>>().Value;
        var kp = sp.GetRequiredService<IKeyProvider>();
        return new JwtTokenBuilder(opts, kp, policyName, tenantId);
    }

    public JwtTokenBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public JwtTokenBuilder WithClaim(string name, object value)
    {
        _claims[name] = value;
        return this;
    }

    public JwtTokenBuilder WithAudience(string audience)
    {
        _audienceOverride = audience;
        return this;
    }

    public JwtTokenBuilder ExpiresIn(TimeSpan duration)
    {
        _expiresOverride = DateTimeOffset.UtcNow.Add(duration);
        return this;
    }

    public async ValueTask<string> SignAsync(CancellationToken ct = default)
    {
        var policy = _options.TokenPolicies[_policyName];
        var keyMat = await _keyProvider.GetCurrentSigningKeyAsync(_policyName, _tenantId, ct);

        var now = DateTimeOffset.UtcNow;
        var expires = _expiresOverride ?? now.Add(policy.ExpireTime);
        var audience = _audienceOverride ?? _tenantId ?? "default-aud";
        var tokenType = policy.TypHeader;

        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _tenantId is not null ? $"https://auth.{_tenantId}.local" : "https://auth.local",
            Audience = audience,
            Expires = expires.UtcDateTime,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Subject = BuildIdentity(),
            SigningCredentials = new SigningCredentials(keyMat.Key, policy.SigningAlgorithm)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = true }
            },
            TokenType = tokenType
        };

        if (policy.IncludeJwtId && !_claims.ContainsKey(JwtRegisteredNames.Jti))
            _claims[JwtRegisteredNames.Jti] = Guid.NewGuid().ToString("N");

        if (policy.EncryptToken)
            throw new NotImplementedException("JWE support will be added in a future increment.");

        return handler.CreateToken(descriptor);

        ClaimsIdentity BuildIdentity()
        {
            var claims = _claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)).ToList();

            if (_subject is { Length: > 0 })
                claims.Add(new Claim(JwtRegisteredNames.Sub, _subject));

            return new ClaimsIdentity(claims, "JWT");
        }
    }
}
