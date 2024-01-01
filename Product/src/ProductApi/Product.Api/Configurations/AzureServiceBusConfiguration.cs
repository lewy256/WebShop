namespace ProductApi.Configurations;

public class AzureServiceBusConfiguration {
    public string Section { get; set; } = "AzureServiceBus";
    public string ConnectionString { get; set; }
    public string Queue { get; set; }
}
