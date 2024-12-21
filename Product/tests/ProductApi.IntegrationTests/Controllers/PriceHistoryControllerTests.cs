using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using ProductApi.Shared;
using ProductApi.Shared.LinkModels.PriceHistory;
using ProductApi.Shared.PriceHistoryDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

[Collection("FixtureCollection")]
public class PriceHistoryControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public PriceHistoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "1.0");
        _productApiFactory = productApiFactory;
    }

    public async Task<List<PriceHistory>> SeedAsync(int count, bool savePriceHistory) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var priceHistoriesToDelete = await context.PriceHistory.ToListAsync();

        context.PriceHistory.RemoveRange(priceHistoriesToDelete);

        var faker = new PriceHistoryFaker();

        await context.Product.AddAsync(faker.Product);

        var priceHistories = new List<PriceHistory>();

        if(savePriceHistory) {
            priceHistories = faker.Generate(count);
            await context.PriceHistory.AddRangeAsync(priceHistories);
        }
        else {
            priceHistories = faker.Generate(count);
        }

        await context.SaveChangesAsync();

        return priceHistories;
    }


    [Fact]
    public async Task GetPriceHistories_ReturnsOkLinks() {
        var priceHistory = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.First().ProductId}/price-histories");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedPriceHistoryEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreatePriceHistory_WithValidModel_ReturnsCreatedStatus() {
        var priceHistories = await SeedAsync(1, false);
        var priceHistory = priceHistories.First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/price-histories", createPriceHistoryDto);
        var response = await postResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var priceHistories = await SeedAsync(1, false);
        var priceHistory = priceHistories.First();
        priceHistory.StartDate = new DateTime(2023, 12, 10);
        priceHistory.EndDate = new DateTime(2005, 10, 5);
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/price-histories", createPriceHistoryDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var priceHistories = await SeedAsync(1, false);

        var response = await _client.PostAsJsonAsync($"/api/products/{priceHistories.First().ProductId}/price-histories", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, false);
        var priceHistory = priceHistories.First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/price-histories", createPriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithValidId_ReturnsOkResult() {
        var priceHistories = await SeedAsync(1, true);
        var priceHistory = priceHistories.First();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/price-histories/{priceHistory.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}/price-histories/{priceHistories.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{priceHistories.First().ProductId}/price-histories/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistories_ReturnsOkResult() {
        var priceHistories = await SeedAsync(2, true);
        var expectedResponse = priceHistories.Adapt<IEnumerable<PriceHistoryDto>>();

        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        var getResponse = await _client.GetAsync($"/api/products/{priceHistories.First().ProductId}/price-histories");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PriceHistoryDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Theory]
    [InlineData("?pageSize=10&pageNumber=3&minPrice=4&maxPrice=10&startDate=12/24/2000&endDate=12/24/2023")]
    [InlineData("?orderBy=priceValue desc")]
    public async Task GetPriceHistories_WithValidParameters_ReturnsOkResult(string queryParams) {
        var priceHistories = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{priceHistories.First().ProductId}/price-histories" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("?pageSize=60&pageNumber=30", 1)]
    [InlineData("?minPrice=10&maxPrice=5", 1)]
    [InlineData("?startDate=12/24/2023&endDate=12/24/2000", 1)]
    [InlineData("?orderBy=priceValue,endDate desc", 1)]
    [InlineData("?orderBy=id desc", 1)]
    public async Task GetPriceHistories_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams, int count) {
        var priceHistories = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{priceHistories.First().ProductId}/price-histories" + queryParams);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(count);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithValidModel_ReturnsNoContent() {
        var priceHistories = await SeedAsync(1, true);
        var priceHistory = priceHistories.First();
        priceHistory.PriceValue = 44.44M;
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/price-histories/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var priceHistories = await SeedAsync(1, true);
        var priceHistory = priceHistories.First();
        priceHistory.StartDate = new DateTime(2023, 12, 10);
        priceHistory.EndDate = new DateTime(2005, 10, 5);
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/price-histories/{priceHistory.Id}", updatePriceHistoryDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistories.First().ProductId}/price-histories/{priceHistories.First().Id}", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);
        var priceHistory = priceHistories.First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}/price-histories/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);
        var priceHistory = priceHistories.First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/price-histories/{Guid.NewGuid()}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePriceHistory_WithValidId_ReturnsNoContent() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{priceHistories.First().ProductId}/price-histories/{priceHistories.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/price-histories/{priceHistories.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeletePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistories = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{priceHistories.First().ProductId}/price-histories/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}