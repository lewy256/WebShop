using Xunit;

namespace OrderApi.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<OrderApiFactory>, IAsyncLifetime {
    protected HttpClient _client { get; set; }
    private readonly Func<Task> _dbReset;
    public BaseIntegrationTest(OrderApiFactory factory) {
        _client = factory.CreateClient();
        _dbReset = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() {
        await _dbReset();
    }
}
