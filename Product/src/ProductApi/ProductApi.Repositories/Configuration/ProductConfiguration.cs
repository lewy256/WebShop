/*using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ProductApi.Repository.Configuration;

public class ProductConfiguration {
    //i will change to private later
    public static async Task<ProductRepository> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection) {
        var databaseName = configurationSection["DatabaseName"];
        var containerName = configurationSection["ProductContainer:ContainerName"];
        var partitionKey = configurationSection["ProductContainer:PartitionKey"];
        var account = configurationSection["Account"];
        var key = configurationSection["Key"];
        var client = new CosmosClient(account, key);
        // var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        // await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
        var cosmosDbService = new ProductRepository(client, databaseName, containerName, partitionKey);
        return cosmosDbService;
    }
}
*/