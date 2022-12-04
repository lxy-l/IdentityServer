using System;
using Microsoft.IdentityModel.Tokens;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServer.Config
{
	public static class OpeniddictConfig
	{
		public static void AddOpeniddictConfig(this IServiceCollection Services)
		{
            Services.AddOpenIddict()
                        //.AddCore(options =>
                        //{
                        //    options.UseEntityFrameworkCore()
                        //           .UseDbContext<ApplicationDbContext>();
                        //})
                        .AddServer(options =>
                        {
                            options.SetAuthorizationEndpointUris("/connect/authorize", "/connect/authorize/callback")
                            .SetDeviceEndpointUris("/device")
                            .SetIntrospectionEndpointUris("/connect/introspect")
                            .SetLogoutEndpointUris("/connect/logout")
                            .SetRevocationEndpointUris("/connect/revocat")
                            .SetTokenEndpointUris("/connect/token")
                            .SetUserinfoEndpointUris("/connect/userinfo")
                            .SetVerificationEndpointUris("/connect/verify");
                            //options.AllowAuthorizationCodeFlow();
                            options.AllowAuthorizationCodeFlow()
                            .AllowHybridFlow()
                            .AllowImplicitFlow()
                            .AllowPasswordFlow()
                            .AllowClientCredentialsFlow()
                            .AllowRefreshTokenFlow()
                            .AllowDeviceCodeFlow()
                            .AllowNoneFlow();
                            options.RegisterScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Phone, Scopes.Roles, Scopes.Address, Scopes.OfflineAccess);
                            options.AddEncryptionKey(new SymmetricSecurityKey(
                                Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));
                            options.AddDevelopmentSigningCertificate();
                            options.UseAspNetCore()
                                .EnableAuthorizationEndpointPassthrough()
                                .EnableTokenEndpointPassthrough()
                                .EnableUserinfoEndpointPassthrough()
                                .EnableLogoutEndpointPassthrough()
                                .EnableVerificationEndpointPassthrough()
                                .EnableStatusCodePagesIntegration()
                                .DisableTransportSecurityRequirement();
                        }).AddValidation(options =>
                        {
                            options.AddAudiences("Roy");
                            options.UseLocalServer();
                            options.UseAspNetCore();
                        });
            Services.AddAuthorization();
        }

    }
}

