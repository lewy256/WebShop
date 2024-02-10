namespace ProductApi.Configurations;

public class RedisConfiguration {
    public const string Section = "Redis";
    public string ConnectionString { get; init; }
    public string InstanceName { get; init; }
}
