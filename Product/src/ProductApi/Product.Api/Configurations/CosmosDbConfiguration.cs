namespace ProductApi.Configurations;

public class CosmosDbConfiguration {
    public string Section { get; set; } = "CosmosDB";
    public string AccountEndpoint { get; set; }
    public string AccountKey { get; set; }
    public string DatabaseName { get; set; }
}
