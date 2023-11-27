using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductApi.Model;
using Testcontainers.CosmosDb;
using Xunit;

namespace ProductApi.IntegrationTests;

public class ProductApiFactory : WebApplicationFactory<Program>, IAsyncLifetime {
    private readonly CosmosDbContainer _container = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
        .WithName("azure-cosmos-emulator")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_ARGS", "/Port=8081")
        .WithPortBinding(8081, 8081)
        .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "30")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "true")
        .WithCleanUp(true)
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            services.RemoveAll<DbContextOptions<ProductContext>>();
            services.RemoveAll<ProductContext>();


            services.AddDbContext<ProductContext>(options => {
                options.UseCosmos(
                    "https://localhost:8081/",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Shop",
                    (clientOptions) => {
                        clientOptions.HttpClientFactory(() => {
                            HttpMessageHandler httpMessageHandler = new HttpClientHandler() {
                                //ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                                ServerCertificateCustomValidationCallback =
                                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                            };

                            return new HttpClient(httpMessageHandler);
                        });
                        clientOptions.ConnectionMode(ConnectionMode.Gateway);
                    }
                );
            });
        });
    }

    public async Task InitializeAsync() {
        await _container.StartAsync();
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();
        await context.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() {
        await _container.DisposeAsync();
    }
}