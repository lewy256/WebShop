namespace BasketApi.Configurations;

public class AzureServiceBusConfiguration {
    public const string Section = "AzureServiceBus";
    public string ConnectionString { get; init; }
}
