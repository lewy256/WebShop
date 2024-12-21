namespace ProductApi.Infrastructure.Configurations.AppSettings;

public class CosmosDbConfiguration {
    public const string Section = "CosmosDB";
    public string AccountEndpoint { get; init; }
    public string AccountKey { get; init; }
    public string DatabaseName { get; init; }
}
