using IdentityApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Testcontainers.MsSql;
using Xunit;

namespace IdentityApi.IntegrationTests;

public class IdentityApiFactory : WebApplicationFactory<Program>, IAsyncLifetime {
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .Build();

    private Respawner _respawner = null!;
    public HttpClient Client { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            services.RemoveAll<DbContextOptions<IdentitytContext>>();
            services.RemoveAll<IdentitytContext>();
            services.AddDbContext<IdentitytContext>(x =>
            x.UseSqlServer(_container.GetConnectionString()));
        });

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    }

    public async Task InitializeAsync() {
        await _container.StartAsync();
        Client = CreateClient();

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentitytContext>();
        await context.Database.EnsureCreatedAsync();

        _respawner = await Respawner.CreateAsync(_container.GetConnectionString(), new RespawnerOptions {
            TablesToIgnore = ["AspNetRoles"]
        });
    }

    public async Task ResetDatabaseAsync() {
        await _respawner.ResetAsync(_container.GetConnectionString());
    }

    async Task IAsyncLifetime.DisposeAsync() {
        await _container.DisposeAsync();
    }
}
