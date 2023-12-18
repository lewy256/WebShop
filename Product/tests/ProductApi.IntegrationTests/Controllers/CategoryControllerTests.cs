using Bogus;
using FluentAssertions;
using ProductApi.Model.Entities;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class CategoryControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;

    private readonly Faker<Category> _fakerGenerator =
        new Faker<Category>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.CategoryName, f => f.Commerce.ProductName());


    public CategoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_WithValidModel_ReturnsCreatedStatus() {
        var category = _fakerGenerator.Generate();

        var postResponse = await _client.PostAsJsonAsync("/api/categories", category);
        var getResponse = await postResponse.Content.ReadFromJsonAsync<Category>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.Id.Should().NotBeEmpty();
        getResponse.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task GetCategory_WithValidId_ReturnsOkResult() {
        var category = _fakerGenerator.Generate();
        var dupa = await _client.PostAsJsonAsync("/api/categories", category);

        var getResponse = await _client.GetAsync($"/api/categories/{category.Id}");
        var categoryResponse = await getResponse.Content.ReadFromJsonAsync<Category>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        categoryResponse.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var category = _fakerGenerator.Generate();
        var putResponse = await _client.PutAsJsonAsync("/api/categories", category);

        putResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }


    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent() {
        var category = _fakerGenerator.Generate();
        var postResponse = await _client.PostAsJsonAsync("/api/categories", category);
        var getResponse = await postResponse.Content.ReadFromJsonAsync<Category>();

        var deleteResponse = await _client.DeleteAsync($"/api/categories/{getResponse.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
