using OpenIddict.Abstractions;

using OpenIddictServer.Data;

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

            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            await CreateApplicationsAsync();
            await CreateScopesAsync();

            async Task CreateApplicationsAsync()
            {
                 var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
