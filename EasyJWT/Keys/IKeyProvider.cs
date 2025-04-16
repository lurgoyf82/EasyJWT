namespace EasyJWT.Keys;

/// <summary>
/// Astrazione sullo storage delle chiavi e sul meccanismo di firma.
/// </summary>
public interface IKeyProvider
{
    /// <summary>
    /// Restituisce la chiave corrente con cui firmare token secondo la policy specificata.
    /// </summary>
    ValueTask<KeyMaterial> GetCurrentSigningKeyAsync(string policyName, string? tenantId = null, CancellationToken ct = default);

    /// <summary>
    /// Recupera la chiave data l’identità del tenant (opzionale) e il <c>kid</c> presente nel token.
    /// </summary>
    ValueTask<KeyMaterial?> GetKeyByIdAsync(string keyId, string? tenantId = null, CancellationToken ct = default);
}
