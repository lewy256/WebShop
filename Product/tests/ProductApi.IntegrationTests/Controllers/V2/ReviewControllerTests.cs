using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Shared.Model.ReviewDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V2;

[Collection("FixtureCollection")]
public class ReviewControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ReviewControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "2.0");
        _productApiFactory = productApiFactory;
    }

    public async Task<List<Review>> SeedAsync(int count, bool saveReview) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var reviewsToDelete = await context.Review.ToListAsync();

        context.Review.RemoveRange(reviewsToDelete);

        var faker = new ReviewFaker();

        await context.Product.AddAsync(faker.Product);

        var reviews = new List<Review>();

        if(saveReview) {
            reviews = faker.Generate(count);
            await context.Review.AddRangeAsync(reviews);
        }
        else {
            reviews = faker.Generate(count);
        }

        await context.SaveChangesAsync();

        return reviews;
    }

    [Fact]
    public async Task GetReviews_ReturnsOkResult() {
        var reviews = await SeedAsync(2, true);
        var expectedResponse = reviews.Adapt<IEnumerable<ReviewDto>>();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/products/{reviews.First().ProductId}/reviews");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ReviewDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetReviews_ReturnsOkLinks() {
        var review = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/products/{review.First().ProductId}/reviews");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedReviewEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }
}
