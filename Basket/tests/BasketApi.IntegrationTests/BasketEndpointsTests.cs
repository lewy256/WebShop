using BasketApi.Responses;
using BasketApi.Shared;
using FluentAssertions;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.Redis;
using Xunit;

namespace BasketApi.IntegrationTests;

public class BasketEndpointsTests : IAsyncLifetime {
    private HttpClient _client = null!;

    private readonly RedisContainer _redisContainer =
        new RedisBuilder()
            .WithImage("redis:7.0")
            .Build();

    [Fact]
    public async Task UpdateBasket_WithValidModel_ReturnsOk() {
        var basket = new BasketFaker().Generate();
        var updateBasketDto = basket.Adapt<UpdateBasketDto>();
        var expectedResponse = basket.Adapt<BasketDto>();

        var response = await _client.PostAsJsonAsync("/api/basket", updateBasketDto);
        var basketDto = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketDto.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(b => b.TotalPrice));
    }

    [Fact]
    public async Task UpdateBasket_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/basket", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBasket_WithInvalidModel_ReturnsUnprocessableEntity() {
        var basket = new BasketFaker().Generate();
        basket.Id = default;

        var response = await _client.PostAsJsonAsync("/api/basket", basket);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CheckoutBasket_WithInvalidId_ReturnsBadRequest() {
        var response = await _client.PostAsync($"/api/basket/{Guid.NewGuid()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckoutBasket_WithValidId_ReturnsAccepted() {
        var basket = new BasketFaker().Generate();
        var createBasketDto = basket.Adapt<UpdateBasketDto>();
        await _client.PostAsJsonAsync($"/api/basket", createBasketDto);

        var response = await _client.PostAsync($"/api/basket/{basket.Id}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetBasket_WithValidId_ReturnsOk() {
        var basket = new BasketFaker().Generate();
        var createBasketDto = basket.Adapt<UpdateBasketDto>();
        var expectedResponse = basket.Adapt<BasketDto>();
        await _client.PostAsJsonAsync("/api/basket", createBasketDto);

        var response = await _client.GetAsync($"/api/basket/{basket.Id}");
        var basketDto = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketDto.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(b => b.TotalPrice));
    }


    [Fact]
    public async Task GetBasket_WithInvalidId_ReturnsOk() {
        var expectedResponse = new BasketDto() {
            Id = Guid.NewGuid(),
            Items = []
        };

        var response = await _client.GetAsync($"/api/basket/{expectedResponse.Id}");
        var basketDto = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketDto.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(b => b.TotalPrice));
    }


    [Fact]
    public async Task DeleteBasket_WithValidId_ReturnsNoContent() {
        var basket = new BasketFaker().Generate();
        await _client.PostAsJsonAsync("/api/basket", basket);

        var response = await _client.DeleteAsync($"/api/basket/{basket.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBasket_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/basket/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync() {
        await _redisContainer.StartAsync();

        var waf = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    services.AddMassTransitTestHarness();

                    services.AddStackExchangeRedisCache(options => {
                        options.Configuration = _redisContainer.GetConnectionString();
                    });

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });

                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            });

        _client = waf.CreateClient();
    }

    public async Task DisposeAsync() {
        await _redisContainer.DisposeAsync();
    }
}


