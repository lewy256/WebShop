using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Categories;
using ProductApi.Shared.Model.CategoryDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class CategoryControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public CategoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public List<Category> Seed(int count) {
        using var scope = _productApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var categories = new List<Category>();

        for(int i = 0; i < count; i++) {
            categories.Add(SeedData.CategoryGenerator());
        }

        context.Category.AddRange(categories);
        context.SaveChanges();

        return categories;
    }

    [Fact]
    public async Task CreateCategory_WithValidModel_ReturnsCreatedStatus() {
        var category = Seed(1).First();
        var createCategoryDto = category.Adapt<CreateCategoryDto>();
        var categoryDto = category.Adapt<CategoryDto>();

        var postResponse = await _client.PostAsJsonAsync("/api/categories", createCategoryDto);
        var response = await postResponse.Content.ReadFromJsonAsync<CategoryDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(categoryDto, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreateCategory_WithInvalidModel_ReturnsBadRequest() {
        CreateCategoryDto createCategoryDto = null;

        var response = await _client.PostAsJsonAsync("/api/categories", createCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategory_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var category = Seed(1).First();
        var createCategoryDto = category.Adapt<CreateCategoryDto>();
        createCategoryDto.CategoryName = "";

        var response = await _client.PostAsJsonAsync("/api/categories", createCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetCategory_WithValidId_ReturnsOkResult() {
        var category = Seed(1).First();
        var categoryDto = category.Adapt<CategoryDto>();

        var getResponse = await _client.GetAsync($"/api/categories/{category.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<CategoryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        categoryDto.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetCategory_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategories_ReturnsOkResult() {
        Seed(2);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync("/api/categories");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCategories_ReturnsOkLinks() {
        Seed(2);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync("/api/categories");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedCategoryEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateCategory_WithValidModel_ReturnsNoContent() {
        var category = Seed(1).First();
        var updateCategoryDto = category.Adapt<UpdateCategoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{category.Id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdateCategory_WithInvalidModel_ReturnsBadRequest() {
        var id = Guid.NewGuid();
        UpdateCategoryDto updateCategoryDto = null;

        var response = await _client.PutAsJsonAsync($"/api/categories/{id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var category = Seed(1).First();
        var updateCategoryDto = category.Adapt<UpdateCategoryDto>();
        updateCategoryDto.CategoryName = "";

        var response = await _client.PutAsJsonAsync($"/api/categories/{category.Id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidId_ReturnsNotFound() {
        var category = Seed(1).First();
        var updateCategoryDto = category.Adapt<UpdateCategoryDto>();
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"/api/categories/{id}", updateCategoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent() {
        var category = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/categories/{category.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
