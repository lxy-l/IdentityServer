﻿using Microsoft.AspNetCore.Identity;

using OpenIddict.Abstractions;
using OpenIddictServerAspNetIdentity.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServerAspNetIdentity
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await CreateApplicationsAsync();
            await CreateScopesAsync();

            async Task CreateApplicationsAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
                if (await manager.FindByClientIdAsync("postman", cancellationToken) is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "postman",
                        ClientSecret = "postman-secret",
                        DisplayName = "Postman",
                        RedirectUris = { new Uri("https://localhost:7059/account/login") },
                        Permissions =
                        {
                           Permissions.Endpoints.Authorization,
                           Permissions.Endpoints.Token,

                           Permissions.GrantTypes.AuthorizationCode,
                           Permissions.GrantTypes.ClientCredentials,
                           Permissions.GrantTypes.RefreshToken,

                           Permissions.Prefixes.Scope + "api",
                           Permissions.ResponseTypes.Code
                        }
                    }, cancellationToken);
                }

                if (await manager.FindByClientIdAsync("app", cancellationToken) is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "app",
                        ClientSecret = "app-secret",
                        DisplayName = "app",
                        ConsentType= GrantTypes.Password,
                        //Requirements
                        //Type
                        Permissions =
                        {
                           Permissions.Endpoints.Authorization,
                           Permissions.Endpoints.Token,
                           

                           Permissions.GrantTypes.AuthorizationCode,
                           Permissions.GrantTypes.ClientCredentials,
                           Permissions.GrantTypes.Password,
                           Permissions.GrantTypes.RefreshToken,

                           Permissions.Prefixes.Scope + "api",
                           Permissions.ResponseTypes.Code,
                           Permissions.ResponseTypes.IdToken,
                           Permissions.ResponseTypes.CodeIdTokenToken
                        }
                    }, cancellationToken);
                }

            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
                if (await manager.FindByNameAsync("api", cancellationToken) is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        Name= "api",
                        DisplayName = "api",
                        Description="测试Api"
                    }, cancellationToken);
                }
            }


        }



        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
