using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Globalization;
using OpenIddictServer.Data;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;
using Microsoft.EntityFrameworkCore;

namespace OpenIddictServer
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
                        //RedirectUris = { new Uri("https://localhost:7059/account/login") },
                        Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Permissions.ResponseTypes.Code
                    }
                    }, cancellationToken);
                }


                if (await manager.FindByClientIdAsync("console") == null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "console",
                        ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                        DisplayName = "My client application",
                        Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
                    });
                }

                if (await manager.FindByClientIdAsync("test")==null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "test",
                        DisplayName= "test",
                        RedirectUris= {new Uri("http://localhost")},
                        Permissions = 
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.ClientCredentials,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.Prefixes.Scope+"api",
                            Permissions.Prefixes.Scope+"openid",
                            Permissions.Prefixes.Scope+"offline_access",
                            Permissions.ResponseTypes.Code
                        }
                    });
                }

                if (await manager.FindByClientIdAsync("console_app") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "console_app",
                        RedirectUris =
                        {
                            new Uri("http://localhost:5213/")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            Permissions.Prefixes.Scope + "api1",
                            Permissions.Prefixes.Scope + "api2"
                        }
                    });
                }

                if (await manager.FindByClientIdAsync("resource_server_1") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "resource_server_1",
                        ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    });
                }
            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

                if (await manager.FindByNameAsync("api1") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        Name = "api1",
                        Resources =
                        {
                            "resource_server_1"
                        }
                    });
                }

                if (await manager.FindByNameAsync("api2") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        Name = "api2",
                        Resources =
                        {
                            "resource_server_2"
                        }
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
