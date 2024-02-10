using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Categories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V2;

[Collection("FixtureCollection")]
public class CategoryControllerTests {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public CategoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "2.0");
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
    public async Task GetCategories_ReturnsOkLinks() {
        await SeedAsync(2);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync("/api/categories");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedCategoryEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNull();
        response.Value.Should().NotContainNulls();
        response.Value.Should().HaveCount(2);
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCategories_ReturnsOkResult() {
        var expectedResponse = await SeedAsync(2);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync("/api/categories");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<Category>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }
}
