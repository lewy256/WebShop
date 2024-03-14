namespace BasketApi.Configurations;

public class RedisConfiguration {
    public const string Section = "Redis";
    public string RedisCache { get; init; }
    public string InstanceName { get; init; }
}
