namespace ProductApi.Service.Configurations;
public class AzureBlobStorageConfiguration {
    public const string Section = "AzureBlobStorage";
    public string ConnectionString { get; init; }
    public string Container { get; init; }
}
