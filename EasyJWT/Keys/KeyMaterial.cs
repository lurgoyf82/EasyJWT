using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Keys;

/// <summary>
/// Incapsula la chiave adoperata per firmare o convalidare un JWT e i metadati associati.
/// </summary>
public sealed record KeyMaterial
{
    public KeyMaterial(SecurityKey key,
                       string keyId,
                       string algorithm,
                       DateTimeOffset createdAt,
                       DateTimeOffset? expiresAt = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        KeyId = keyId ?? throw new ArgumentNullException(nameof(keyId));
        Algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>Chiave crittografica, simmetrica o asimmetrica.</summary>
    public SecurityKey Key { get; }

    /// <summary>Identificatore univoco della chiave; viene copiato nell’header JWT come <c>kid</c>.</summary>
    public string KeyId { get; }

    /// <summary>Algoritmo previsto per questa chiave, es. <c>RS256</c> o <c>ES256</c>.</summary>
    public string Algorithm { get; }

    /// <summary>Istantanea di creazione o attivazione della chiave.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Istantanea di scadenza; se null, la chiave è valida finché non viene esplicitamente revocata.</summary>
    public DateTimeOffset? ExpiresAt { get; }

    /// <inheritdoc/>
    public override string ToString()
        => $"{KeyId} ({Algorithm}) created {CreatedAt:u}";
}
