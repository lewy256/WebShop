namespace ProductApi.Configurations;

public class AzureServiceBusConfiguration {
    public const string Section = "AzureServiceBus";
    public string ConnectionString { get; init; }
    public string Queue { get; init; }
}
