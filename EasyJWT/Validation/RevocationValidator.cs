using System.Security.Claims;
using EasyJWT.Configuration;

namespace EasyJWT.Validation
{
    /// <summary>
    /// Validator di default che non fa nulla (override per logica di revoca).
    /// </summary>
    public sealed class RevocationValidator : IJwtValidator
    {
        public ValueTask ValidateAsync(ClaimsPrincipal principal,
                                       string policyName,
                                       string? tenantId = null,
                                       CancellationToken ct = default)
        {
            // No-op: qui potrai inserire il controllo su jti in blacklist.
            return ValueTask.CompletedTask;
        }
    }
}
