/*using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using ProductApi.Interfaces;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model;
using System.Linq.Expressions;

namespace ProductApi.Repository;



public class ProductRepository : IProductRepository {
    private readonly string _partitionKey;
    private readonly Container _container;

    public ProductRepository(CosmosClient cosmosDbClient, string databaseName, string containerName, string partitionKey) {
        _partitionKey = partitionKey;
        _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }


    public async Task AddAsync(Product product) {
        // var id = item.GetType().GetProperty("Id").GetValue(item, null).ToString();
        var item = await GetByIdAsync(product.Id);

        var partitionKeyValue = new PartitionKey(item.CategoryID);

        await _container.CreateItemAsync(item, partitionKeyValue);
    }

    public async Task DeleteAsync(string id) {
        var item = await GetByIdAsync(id);

        var partitionKeyValue = new PartitionKey(item.CategoryID);

        await _container.DeleteItemAsync<Product>(id, partitionKeyValue);
    }

    public async Task<Product> GetByIdAsync(string id) {
        double totalRUsConsumed = 0;

        var query = _container
            .GetItemLinqQueryable<Product>()
            .Where(item => item.Id.Equals(id))
            .ToFeedIterator();

        var response = await query.ReadNextAsync();

        totalRUsConsumed = response.RequestCharge;

        var product = response.SingleOrDefault();

        //var partitionKeyValue = item.GetType().GetProperty(_partitionKey).GetValue(item, null).ToString();

        return product;

    }

    public async Task<IEnumerable<Product>> GetMultipleAsync(Expression<Func<Product, bool>> filter, ProductParemeters productParameters) {
        double totalRUsConsumed = 0;

        var query = _container
            .GetItemLinqQueryable<Product>()
            .Where(filter)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .ToFeedIterator<Product>();



        var results = new List<Product>();



        while (query.HasMoreResults) {
            var response = await query.ReadNextAsync();
            totalRUsConsumed = response.RequestCharge;
            results.AddRange(response);
        }


        return results;
    }

    public async Task UpdateAsync(Product product) {
        var item = await GetByIdAsync(product.Id);

        var dupa = await _container.UpsertItemAsync(product, new PartitionKey(item.CategoryID));

        var cost = dupa.RequestCharge;
    }

    public async Task PatchAsync(string id, Product product) {
        var item = await GetByIdAsync(id);


        var dupa = await _container.PatchItemAsync<Product>(
            id, new PartitionKey(item.CategoryID), new[] { PatchOperation.Replace("/Name", product.Name) });

        var cost = dupa.RequestCharge;
    }

    *//*    private string GetPartitionKeyValue(int id) {

            // var containerResponse = await _container.ReadContainerAsync();


            var query = _container
                .GetItemLinqQueryable<T>()
                .Where<T>(item => item.Id == id)
                .ToFeedIterator<T>();

            var partitionKeyValue = item.GetType().GetProperty(_partitionKey).GetValue(item, null).ToString();

            var query = _container.GetItemLinqQueryable<T>().Where(i => i.GetType().GetProperty("id").GetValue().Equals(id));

            return partitionKeyValue;

            //return containerResponse.Resource.PartitionKeyPath;
        }
    *//*

}
*/