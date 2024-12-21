using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Configurations.AppSettings;
using Testcontainers.Azurite;
using Testcontainers.CosmosDb;
using Xunit;

namespace ProductApi.IntegrationTests;

public class ProductApiFactory : WebApplicationFactory<Program>, IAsyncLifetime {
    private readonly CosmosDbContainer _cosmosContainer = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        .WithName("azure-cosmos-emulator")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_ARGS", "/Port=8081")
        .WithPortBinding(8081, 8081)
        .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "5")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
        .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
        .Build();

    private readonly AzuriteContainer _storageContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:3.33.0")
        .WithName("azurite")
        .Build();

    private string _blobContainerName { get; init; } = "images";

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            services.RemoveAll<DbContextOptions<ProductContext>>();
            services.RemoveAll<ProductContext>();

            services.AddMassTransitTestHarness();

            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

            services.AddDbContext<ProductContext>(options => {
                options.UseCosmos(
                    _cosmosContainer.GetConnectionString(),
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

            services.Configure<AzureBlobStorageConfiguration>(opts => {
                opts.ConnectionString = _storageContainer.GetConnectionString();
                opts.Container = _blobContainerName;
            });
        });
    }

    public async Task InitializeAsync() {
        await _storageContainer.StartAsync();
        await _cosmosContainer.StartAsync();

        var blobServiceClient = new BlobServiceClient(_storageContainer.GetConnectionString());
        await blobServiceClient.CreateBlobContainerAsync(_blobContainerName);
    }

    async Task IAsyncLifetime.DisposeAsync() {
        await _cosmosContainer.DisposeAsync();
        await _storageContainer.DisposeAsync();
    }
}