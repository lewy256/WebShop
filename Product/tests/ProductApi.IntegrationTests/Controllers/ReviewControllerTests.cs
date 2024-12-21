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
using ProductApi.Shared.LinkModels.Reviews;
using ProductApi.Shared.ReviewDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

[Collection("FixtureCollection")]
public class ReviewControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ReviewControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
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

    [Fact]
    public async Task CreateReview_WithValidModel_ReturnsCreatedStatus() {
        var reviews = await SeedAsync(1, false);
        var review = reviews.First();
        var createReviewDto = review.Adapt<CreateReviewDto>();
        var expectedResponse = review.Adapt<ReviewDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/products/{review.ProductId}/reviews", createReviewDto);
        var response = await postResponse.Content.ReadFromJsonAsync<ReviewDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.UserName.Should().NotBeNullOrEmpty();
        response.Should().BeEquivalentTo(expectedResponse,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.UserName).Excluding(x => x.ReviewDate));
    }

    [Fact]
    public async Task CreateReview_WithInvalidModel_ReturnsBadRequest() {
        var reviews = await SeedAsync(1, false);

        var response = await _client.PostAsJsonAsync($"/api/products/{reviews.First().ProductId}/reviews", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReview_WithInvalidModel_ReturnsUnprocessableEntity() {
        var reviews = await SeedAsync(1, false);
        var review = reviews.First();
        review.Rating = 44;
        var createReviewyDto = review.Adapt<CreateReviewDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{review.ProductId}/reviews", createReviewyDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateReview_WithInvalidId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, false);
        var createReviewyDto = reviews.First().Adapt<CreateReviewDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/reviews", createReviewyDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReview_WithValidId_ReturnsOkResult() {
        var reviews = await SeedAsync(1, true);
        var review = reviews.First();
        var expectedResponse = review.Adapt<ReviewDto>();

        var getResponse = await _client.GetAsync($"/api/products/{review.ProductId}/reviews/{review.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<ReviewDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetReview_WithInvalidProductId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}/reviews/{reviews.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReview_WithInvalidReviewId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/products/{reviews.First().ProductId}/reviews/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReviews_ReturnsOk() {
        var reviews = await SeedAsync(2, true);
        var expectedResponse = reviews.Adapt<IEnumerable<ReviewDto>>();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/products/{reviews.First().ProductId}/reviews");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ReviewDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Theory]
    [InlineData("?pageSize=10&pageNumber=3&rating=4&startDate=12/24/2000&endDate=12/24/2023")]
    [InlineData("?orderBy=rating desc")]
    public async Task GetReviews_WithValidParameters_ReturnsOkResult(string queryParams) {
        var reviews = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{reviews.First().ProductId}/reviews" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("?pageSize=60&pageNumber=30", 1)]
    [InlineData("?rating=8", 1)]
    [InlineData("?startDate=12/24/2023&endDate=12/24/2000", 1)]
    [InlineData("?orderBy=rating,reviewDate desc", 1)]
    [InlineData("?orderBy=id desc", 1)]
    public async Task GetReviews_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams, int count) {
        var reviews = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{reviews.First().ProductId}/reviews" + queryParams);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(count);
    }

    [Fact]
    public async Task UpdateReview_WithValidModel_ReturnsNoContent() {
        var reviews = await SeedAsync(1, true);
        var review = reviews.First();
        review.Rating = 4;
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdateReview_WithInvalidModel_ReturnsBadRequest() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.PutAsJsonAsync($"/api/products/{reviews.First().ProductId}/reviews/{reviews.First().Id}", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidModel_ReturnsUnprocessableEntity() {
        var reviews = await SeedAsync(1, true);
        var review = reviews.First();
        review.Rating = 44;
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{review.Id}", updateReviewDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidProductId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);
        var review = reviews.First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidReviewId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);
        var review = reviews.First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{Guid.NewGuid()}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteReview_WithValidId_ReturnsNoContent() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{reviews.First().ProductId}/reviews/{reviews.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteReview_WithInvalidProductId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/reviews/{reviews.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteReview_WithInvalidReviewId_ReturnsNotFound() {
        var reviews = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/products/{reviews.First().ProductId}/reviews/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}