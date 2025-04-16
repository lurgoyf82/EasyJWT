using System.Security.Claims;

namespace EasyJWT.Validation;

public interface IJwtValidator
{
    ValueTask ValidateAsync(ClaimsPrincipal principal,
                            string policyName,
                            string? tenantId = null,
                            CancellationToken ct = default);
}
