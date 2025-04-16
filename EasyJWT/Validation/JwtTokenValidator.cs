using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EasyJWT.Configuration;
using EasyJWT.Keys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Validation;

/// <summary>
/// Orchestratore della validazione JWT.
/// </summary>
public sealed class JwtTokenValidator
{
    private readonly JwtLibraryOptions _options;
    private readonly IKeyProvider _keyProvider;
    private readonly IEnumerable<IJwtValidator> _custom;
    private readonly ILogger<JwtTokenValidator> _logger;

    public JwtTokenValidator(JwtLibraryOptions options,
                             IKeyProvider keyProvider,
                             IEnumerable<IJwtValidator> custom,
                             ILogger<JwtTokenValidator> logger)
    {
        _options = options;
        _keyProvider = keyProvider;
        _custom = custom;
        _logger = logger;
    }

    public async ValueTask<ClaimsPrincipal> ValidateAsync(string token,
                                                          string policyName,
                                                          string? tenantId = null,
                                                          CancellationToken ct = default)
    {
        var handler = new JwtSecurityTokenHandler();
        var policy = _options.TokenPolicies[policyName];

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireSignedTokens = true,
            ValidAlgorithms = new[] { policy.SigningAlgorithm },
            ClockSkew = TimeSpan.FromSeconds(30),
            IssuerSigningKeyResolver = (_, st, kid, _) =>
            {
                var mat = _keyProvider.GetKeyByIdAsync(kid, tenantId, ct).Result;
                return mat is null ? Array.Empty<SecurityKey>() : new[] { mat.Key };
            }
        };

        ClaimsPrincipal principal;
        try
        {
            principal = handler.ValidateToken(token, parameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Baseline validation failed.");
            throw;
        }

        foreach (var validator in _custom)
            await validator.ValidateAsync(principal, policyName, tenantId, ct);

        return principal;
    }
}
