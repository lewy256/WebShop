namespace BasketApi.Configurations;

public class AzureServiceBusConfiguration {
    public string Section { get; init; } = "AzureServiceBus";
    public string ConnectionString { get; init; }
}
