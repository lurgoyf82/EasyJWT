using EasyJWT.Configuration;
using EasyJWT.Keys;
using EasyJWT.Tokens;
using EasyJWT.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using EasyJWT.Infrastructure;
using System.Security.Cryptography;

namespace EasyJWT.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEasyJwt(this IServiceCollection services,
                                                Action<JwtLibraryOptions> configure)
    {
        services.Configure(configure);
        return services.AddEasyJwtCore();
    }

    public static IServiceCollection AddEasyJwtCore(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<JwtLibraryOptions>, JwtLibraryOptionsSetup>();

        services.AddSingleton<IKeyProvider, InMemoryKeyProvider>(sp =>
        {
            // Provider di default con una chiave RSA generata al volo.
            using var rsa = RSA.Create(2048);
            var key = new RsaSecurityKey(rsa.ExportParameters(true))
            {
                KeyId = Guid.NewGuid().ToString("N")
            };

            var material = new KeyMaterial(key,
                                           key.KeyId!,
                                           SecurityAlgorithms.RsaSha256,
                                           DateTimeOffset.UtcNow);

            return new InMemoryKeyProvider(new[] { material });
        });

        services.AddSingleton<JwtTokenValidator>();
        services.AddSingleton<IJwtValidator, RevocationValidator>();
        services.AddSingleton<JwksEndpoint>(); // implementazione futura in Infrastructure

        return services;
    }
}
