using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EasyJWT.Configuration;

internal sealed class JwtLibraryOptionsSetup : IConfigureOptions<JwtLibraryOptions>
{
    public void Configure(JwtLibraryOptions options)
    {
        if (!options.TokenPolicies.ContainsKey("AccessToken"))
        {
            options.TokenPolicies["AccessToken"] = new TokenPolicyOptions
            {
                Name = "AccessToken",
                SigningAlgorithm = SecurityAlgorithms.EcdsaSha256,
                ExpireTime = TimeSpan.FromMinutes(5),
                IncludeJwtId = true,
                TypHeader = "at+jwt"
            };
        }
    }
}
