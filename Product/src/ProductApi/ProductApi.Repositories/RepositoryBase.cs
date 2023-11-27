/*
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using ProductApi.Interfaces;
using System.Linq.Expressions;

namespace ProductApi.Repository;



public class RepositoryBase<T> : IRepositoryBase<T> where T : class {
    private readonly string _partitionKey;
    private readonly Container _container;

    public RepositoryBase(CosmosClient cosmosDbClient, string databaseName, string containerName, string partitionKey) {
        _partitionKey = partitionKey;
        _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }

    public async Task AddAsync(T item) {
        // var id = item.GetType().GetProperty("Id").GetValue(item, null).ToString();

        await _container.CreateItemAsync(item, new PartitionKey(""));
    }

    public async Task DeleteAsync(string id) {

        await _container.DeleteItemAsync<T>(id, new PartitionKey(""));
    }

    public async Task<T> GetByIdAsync(string id) {
        double totalRUsConsumed = 0;
        //await GetPartitionKeyValue();

        var partitionKey = new PartitionKey("");

        var response = await _container.ReadItemAsync<T>(id, partitionKey);

        totalRUsConsumed = response.RequestCharge;

        return response.Resource;

    }

    public async Task<IEnumerable<T>> GetMultipleAsync(Expression<Func<T, bool>> expression) {
        double totalRUsConsumed = 0;

        var query = _container
            .GetItemLinqQueryable<T>()
            .Where(expression)
            .ToFeedIterator();


        var results = new List<T>();



        while (query.HasMoreResults) {
            var response = await query.ReadNextAsync();
            totalRUsConsumed = response.RequestCharge;
            results.AddRange(response);
        }


        return results;
    }

    public async Task UpdateAsync(T item) {
        await _container.UpsertItemAsync(item, new PartitionKey(""));
    }

    public async Task PatchAsync(string id, T item) {
        await _container.PatchItemAsync<T>(
            id,
            new PartitionKey(""),
            new[] { PatchOperation.Replace("/price", 355.45) }
        );
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
        }*//*


    private string GetPartitionKeyValue(T item) {


        var partitionKeyValue = item.GetType().GetProperty(_partitionKey).GetValue(item, null).ToString();

        return partitionKeyValue;

    }

    public Task<T> GetByIdAsync(T item, string id) {
        throw new NotImplementedException();
    }
}*/