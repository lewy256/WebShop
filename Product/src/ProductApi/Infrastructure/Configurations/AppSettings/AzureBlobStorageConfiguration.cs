namespace ProductApi.Infrastructure.Configurations.AppSettings;
public class AzureBlobStorageConfiguration {
    public const string Section = "AzureBlobStorage";
    public string ConnectionString { get; set; }
    public string Container { get; set; }
}
