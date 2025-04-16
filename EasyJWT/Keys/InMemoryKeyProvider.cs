using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Keys;

/// <summary>
/// Provider in‑memory, destinato a test o a scenari single‑instance.
/// </summary>
public sealed class InMemoryKeyProvider : IKeyProvider
{
    private readonly ConcurrentDictionary<string, KeyMaterial> _keys;
    private readonly Func<IEnumerable<KeyMaterial>, string, string?, KeyMaterial> _selector;

    public InMemoryKeyProvider(IEnumerable<KeyMaterial> keys,
                               Func<IEnumerable<KeyMaterial>, string, string?, KeyMaterial>? selector = null)
    {
        _keys = new ConcurrentDictionary<string, KeyMaterial>(
                        keys.ToDictionary(k => k.KeyId, k => k));
        _selector = selector ?? DefaultSelector;
    }

    public ValueTask<KeyMaterial> GetCurrentSigningKeyAsync(string policyName,
                                                            string? tenantId = null,
                                                            CancellationToken ct = default)
    {
        var material = _selector(_keys.Values, policyName, tenantId);
        return ValueTask.FromResult(material);
    }

    public ValueTask<KeyMaterial?> GetKeyByIdAsync(string keyId,
                                                   string? tenantId = null,
                                                   CancellationToken ct = default)
    {
        _keys.TryGetValue(keyId, out var material);
        return ValueTask.FromResult<KeyMaterial?>(material);
    }

    private static KeyMaterial DefaultSelector(IEnumerable<KeyMaterial> set,
                                               string policy,
                                               string? tenant)
    {
        // Sceglie la chiave più recente che non sia scaduta.
        return set.Where(k => k.ExpiresAt is null || k.ExpiresAt > DateTimeOffset.UtcNow)
                  .OrderByDescending(k => k.CreatedAt)
                  .First();
    }
}
