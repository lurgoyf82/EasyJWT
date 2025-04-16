using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Configuration;

/// <summary>
/// Opzioni globali della libreria EasyJWT.
/// </summary>
public sealed class JwtLibraryOptions : IValidateOptions<JwtLibraryOptions>
{
    public JwtLibraryOptions()
    {
        TokenPolicies = new Dictionary<string, TokenPolicyOptions>(StringComparer.OrdinalIgnoreCase);
        Tenants = new Dictionary<string, TenantOptions>(StringComparer.OrdinalIgnoreCase);
        GlobalAllowedAlgorithms = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.HmacSha256,
        };
    }

    /// <summary>Algoritmi accettati a livello globale, salvo override per policy o tenant.</summary>
    public ISet<string> GlobalAllowedAlgorithms { get; }

    /// <summary>Configurazioni per ciascuna tipologia di token.</summary>
    public IDictionary<string, TokenPolicyOptions> TokenPolicies { get; }

    /// <summary>Configurazioni per ogni tenant.</summary>
    public IDictionary<string, TenantOptions> Tenants { get; }

    /// <summary>Nome della policy predefinita; deve esistere in <see cref="TokenPolicies"/>.</summary>
    public string DefaultPolicyName { get; set; } = "AccessToken";

    /// <summary>Abilita o disabilita la telemetria OpenTelemetry.</summary>
    public bool EnableTelemetry { get; set; }

    public ValidateOptionsResult Validate(string? name, JwtLibraryOptions options)
    {
        if (!options.TokenPolicies.ContainsKey(options.DefaultPolicyName))
            return ValidateOptionsResult.Fail($"DefaultPolicyName {options.DefaultPolicyName} not found.");
        return ValidateOptionsResult.Success;
    }

    /// <summary>Restituisce la policy richiesta, oppure quella di default.</summary>
    public TokenPolicyOptions GetPolicy(string? name)
    {
        if (name is { Length: > 0 } && TokenPolicies.TryGetValue(name, out var policy))
            return policy;
        return TokenPolicies[DefaultPolicyName];
    }
}
