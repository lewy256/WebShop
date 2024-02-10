using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.CategoryDtos;
using ProductApi.Shared.Model.Responses;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V1;

[Collection("FixtureCollection")]
public class CategoryControllerTests {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public CategoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public async Task<List<Category>> SeedAsync(int count) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var categoriesToDelete = await context.Category.ToListAsync();

        context.Category.RemoveRange(categoriesToDelete);

        var faker = new CategoryFaker();
        var categories = faker.Generate(count);

        await context.Category.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        return categories;
    }

    [Fact]
    public async Task CreateCategory_WithValidModel_ReturnsCreatedStatus() {
        var categories = await SeedAsync(1);
        var category = categories.First();
        var createCategoryDto = category.Adapt<CreateCategoryDto>();

        var postResponse = await _client.PostAsJsonAsync("/api/categories", createCategoryDto);
        var response = await postResponse.Content.ReadFromJsonAsync<Category>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(category, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreateCategory_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsJsonAsync("/api/categories", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var categories = await SeedAsync(1);
        var category = categories.First();
        category.CategoryName = "";
        var createCategoryDto = category.Adapt<CreateCategoryDto>();

        var response = await _client.PostAsJsonAsync("/api/categories", createCategoryDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCategory_WithValidId_ReturnsOk() {
        var categories = await SeedAsync(1);

        var getResponse = await _client.GetAsync($"/api/categories/{categories.First().Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<Category>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        categories.First().Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetCategory_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategories_ReturnsOk() {
        var categories = await SeedAsync(2);

        var getResponse = await _client.GetAsync("/api/categories");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<Category>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        categories.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task UpdateCategory_WithValidModel_ReturnsNoContent() {
        var categories = await SeedAsync(1);
        var category = categories.First();
        var updateCategoryDto = category.Adapt<UpdateCategoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{category.Id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsJsonAsync($"/api/categories/{Guid.NewGuid()}", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidModel_ReturnsUnprocessableEntity() {
        var categories = await SeedAsync(1);
        var category = categories.First();
        category.CategoryName = "";
        var updateCategoryDto = category.Adapt<UpdateCategoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{category.Id}", updateCategoryDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidId_ReturnsNotFound() {
        var categories = await SeedAsync(1);
        var updateCategoryDto = categories.First().Adapt<UpdateCategoryDto>();
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"/api/categories/{id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent() {
        var categories = await SeedAsync(1);

        var response = await _client.DeleteAsync($"/api/categories/{categories.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
