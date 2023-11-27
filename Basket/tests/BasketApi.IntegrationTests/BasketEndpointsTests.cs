using AutoBogus;
using BasketApi.Entities;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.Redis;
using Xunit;
using Xunit.Abstractions;

namespace BasketApi.IntegrationTests;

public class BasketEndpointsTests : IAsyncLifetime {
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly AutoFaker<Basket> _basketGenerator = new();

    private readonly RedisContainer _redisContainer =
        new RedisBuilder()
        .WithImage("redis:7.0")
        .Build();

    public BasketEndpointsTests(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public async Task CreateBasket_WithValidModel_ReturnsCreatedStatus() {
        var expectedResponse = _basketGenerator.Generate();

        var postResponse = await _client.PostAsJsonAsync("/api/Basket", expectedResponse);
        var basketResponse = await postResponse.Content.ReadFromJsonAsync<Basket>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        basketResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetBasket_WithValidId_ReturnsOkResult() {
        var expectedResponse = _basketGenerator.Generate();

        var postResponse = await _client.PostAsJsonAsync("/api/Basket", expectedResponse);
        var basket = await postResponse.Content.ReadFromJsonAsync<Basket>();

        var getResponse = await _client.GetAsync($"/api/Basket/{basket.Id}");
        var basketResponse = await getResponse.Content.ReadFromJsonAsync<Basket>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        basketResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task DeleteBasket_WithValidId_ReturnsNoContent() {
        var basket = _basketGenerator.Generate();
        var postResponse = await _client.PostAsJsonAsync("/api/Basket", basket);

        var product = await postResponse.Content.ReadFromJsonAsync<Basket>();
        var deleteResponse = await _client.DeleteAsync($"/api/Basket/{product.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }

    [Fact]
    public async Task DeleteBasket_WithInvalidId_ReturnsNotFound() {

        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/Basket/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    public async Task InitializeAsync() {
        await _redisContainer.StartAsync();

        var waf = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureLogging(x => {
                    x.ClearProviders();
                    x.SetMinimumLevel(LogLevel.Debug);
                    x.AddFilter(_ => true);

                    x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(_testOutputHelper));
                });

                builder.ConfigureTestServices(services => {
                    services.AddStackExchangeRedisCache(options => {
                        options.Configuration = _redisContainer.GetConnectionString();
                    });
                });
            });

        _client = waf.CreateClient();
    }

    public async Task DisposeAsync() {
        await _redisContainer.StopAsync();
    }
}

