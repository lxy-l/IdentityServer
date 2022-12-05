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
            //await CreateScopesAsync();

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

            }

            //async Task CreateScopesAsync()
            //{
            //    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            //}
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
