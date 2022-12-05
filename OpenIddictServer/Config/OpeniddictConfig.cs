using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

using OpenIddictServer.Data;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServer.Config
{
    public static class OpeniddictConfig
	{
		public static void AddOpeniddictConfig(this IServiceCollection Services)
		{
            Services.AddOpenIddict()
                        .AddCore(options =>
                        {
                            options.UseEntityFrameworkCore()
                                   .UseDbContext<ApplicationDbContext>();
                        })
                        .AddServer(options =>
                        {
                            options
                            .SetAuthorizationEndpointUris("/connect/authorize")
                            .SetDeviceEndpointUris("/connect/device")
                            .SetIntrospectionEndpointUris("/connect/introspect")
                            .SetRevocationEndpointUris("/connect/revocat")
                            .SetUserinfoEndpointUris("/connect/userinfo")
                            .SetVerificationEndpointUris("/connect/verify")
                            .SetLogoutEndpointUris("/connect/logout")
                            .SetTokenEndpointUris("/connect/token")

                            .AllowDeviceCodeFlow()
                            .AllowClientCredentialsFlow()
                            .AllowAuthorizationCodeFlow()
                            .AllowPasswordFlow()
                            .AllowImplicitFlow()
                            .AllowHybridFlow()
                            .AllowRefreshTokenFlow()

                            .RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles)

                        //    .AddEncryptionKey(new SymmetricSecurityKey(
                        //Convert.FromBase64String("DRjd/nduI3Efze123nvbNUfc/=")))
                            .AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate()
                            //.RequireProofKeyForCodeExchange()

                            .UseAspNetCore()
                                .EnableAuthorizationEndpointPassthrough()
                                .EnableTokenEndpointPassthrough()
                                .EnableUserinfoEndpointPassthrough()
                                .EnableLogoutEndpointPassthrough()
                                .EnableVerificationEndpointPassthrough()
                                .EnableStatusCodePagesIntegration()
                                .DisableTransportSecurityRequirement();
                        })
                        .AddValidation(options =>
                        {
                            //options.AddAudiences("APIResource");
                            options.UseLocalServer();
                            options.UseAspNetCore();
                        });

            Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            //Services
            //    .AddAuthorization()
            //    .AddAuthentication(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            Services.AddHostedService<Worker>();
        }

    }
}

