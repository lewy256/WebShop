using Xunit;

namespace IdentityApi.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IdentityApiFactory>, IAsyncLifetime {
    protected HttpClient HttpClient { get; set; }
    private readonly Func<Task> _dbReset;
    public BaseIntegrationTest(IdentityApiFactory factory) {
        HttpClient = factory.CreateClient();
        _dbReset = factory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() {
        await _dbReset();
    }
}
