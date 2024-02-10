using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.PriceHistoryDtos;
using ProductApi.Shared.Model.Responses;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V1;

[Collection("FixtureCollection")]
public class PriceHistoryControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public PriceHistoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public async Task<List<PriceHistory>> SeedAsync(int count, bool savePriceHistory) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var pricesHistoryToDelete = await context.PriceHistory.ToListAsync();

        context.PriceHistory.RemoveRange(pricesHistoryToDelete);

        var faker = new PriceHistoryFaker();

        await context.Product.AddAsync(faker.Product);

        var pricesHistory = new List<PriceHistory>();

        if(savePriceHistory) {
            pricesHistory = faker.Generate(count);
            await context.PriceHistory.AddRangeAsync(pricesHistory);
        }
        else {
            pricesHistory = faker.Generate(count);
        }

        await context.SaveChangesAsync();

        return pricesHistory;
    }

    [Fact]
    public async Task CreatePriceHistory_WithValidModel_ReturnsCreatedStatus() {
        var pricesHistory = await SeedAsync(1, false);
        var priceHistory = pricesHistory.First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/prices-history", createPriceHistoryDto);
        var response = await postResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var pricesHistory = await SeedAsync(1, false);
        var priceHistory = pricesHistory.First();
        priceHistory.StartDate = new DateTime(12 / 24 / 2023);
        priceHistory.EndDate = new DateTime(12 / 24 / 2020);
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/prices-history", createPriceHistoryDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var pricesHistory = await SeedAsync(1, false);

        var response = await _client.PostAsJsonAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, false);
        var priceHistory = pricesHistory.First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/prices-history", createPriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithValidId_ReturnsOkResult() {
        var pricesHistory = await SeedAsync(1, true);
        var priceHistory = pricesHistory.First();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/prices-history/{priceHistory.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}/prices-history/{pricesHistory.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPricesHistory_ReturnsOkResult() {
        var pricesHistory = await SeedAsync(2, true);
        var expectedResponse = pricesHistory.Adapt<IEnumerable<PriceHistoryDto>>();

        var getResponse = await _client.GetAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PriceHistoryDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Theory]
    [InlineData("pageSize=10&pageNumber=3&minPrice=4&maxPrice=10&startDate=12/24/2000&endDate=12/24/2023")]
    [InlineData("orderBy=priceValue desc")]
    [InlineData("")]
    public async Task GetPricesHistory_WithValidParameters_ReturnsOkResult(string queryParams) {
        var pricesHistory = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageSize=60&pageNumber=30", 1)]
    [InlineData("minPrice=10&maxPrice=5", 1)]
    [InlineData("startDate=12/24/2023&endDate=12/24/2000", 1)]
    [InlineData("orderBy=priceValue,endDate desc", 1)]
    [InlineData("orderBy=id desc", 1)]
    public async Task GetPricesHistory_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams, int count) {
        var pricesHistory = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/?" + queryParams);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(count);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithValidModel_ReturnsNoContent() {
        var pricesHistory = await SeedAsync(1, true);
        var priceHistory = pricesHistory.First();
        priceHistory.PriceValue = 44.44M;
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/prices-history/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var pricesHistory = await SeedAsync(1, true);
        var priceHistory = pricesHistory.First();
        priceHistory.StartDate = new DateTime(12 / 24 / 2023);
        priceHistory.EndDate = new DateTime(12 / 24 / 2020);
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/prices-history/{priceHistory.Id}", updatePriceHistoryDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.PutAsJsonAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/{pricesHistory.First().Id}", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);
        var priceHistory = pricesHistory.First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}/prices-history/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);
        var priceHistory = pricesHistory.First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/prices-history/{Guid.NewGuid()}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePriceHistory_WithValidId_ReturnsNoContent() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/{pricesHistory.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/prices-history/{pricesHistory.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeletePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var pricesHistory = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}