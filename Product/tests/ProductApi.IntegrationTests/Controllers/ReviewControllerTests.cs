using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Shared.Model.ReviewDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class ReviewControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ReviewControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public List<Review> Seed(int count, bool createReview) {
        using var scope = _productApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var products = new List<Product>();
        var reviews = new List<Review>();

        for(int i = 0; i < count; i++) {
            var product = SeedData.ProductGenerator(Guid.NewGuid());
            products.Add(product);
            var review = SeedData.ReviewGenerator(product.Id);
            reviews.Add(review);
        }

        if(createReview) {
            context.Product.AddRange(products);
            context.Review.AddRange(reviews);
        }
        else {
            context.Product.AddRange(products);
        }

        context.SaveChanges();

        return reviews;
    }

    [Fact]
    public async Task CreateReview_WithValidModel_ReturnsCreatedStatus() {
        var review = Seed(1, false).First();
        var createReviewDto = review.Adapt<CreateReviewDto>();
        var expectedResponse = review.Adapt<ReviewDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/products/{review.ProductId}/reviews", createReviewDto);
        var response = await postResponse.Content.ReadFromJsonAsync<ReviewDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(expectedResponse,
            opt => opt.Excluding(x => x.Id).Excluding(x => x.ReviewDate));
    }

    [Fact]
    public async Task CreateReview_WithInvalidModel_ReturnsBadRequest() {
        var review = Seed(1, false).First();
        CreateReviewDto createReviewDto = null;

        var response = await _client.PostAsJsonAsync($"/api/products/{review.ProductId}/reviews", createReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReview_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var review = Seed(1, false).First();
        var createReviewyDto = review.Adapt<CreateReviewDto>();
        createReviewyDto.Rating = 44;

        var response = await _client.PostAsJsonAsync($"/api/products/{review.ProductId}/reviews", createReviewyDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateReview_WithInvalidId_ReturnsNotFound() {
        var review = Seed(1, false).First();
        var createReviewyDto = review.Adapt<CreateReviewDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/reviews", createReviewyDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReview_WithValidId_ReturnsOkResult() {
        var review = Seed(1, true).First();
        var expectedResponse = review.Adapt<ReviewDto>();

        var getResponse = await _client.GetAsync($"/api/products/{review.ProductId}/reviews/{review.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<ReviewDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetReview_WithInvalidProductId_ReturnsNotFound() {
        var review = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}/reviews/{review.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReview_WithInvalidReviewId_ReturnsNotFound() {
        var review = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/products/{review.ProductId}/reviews/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReviews_ReturnsOkResult() {
        var review = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/products/{review.ProductId}/reviews");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ReviewDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetReviews_ReturnsOkLinks() {
        var review = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/products/{review.ProductId}/reviews");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedReviewEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("pageSize=10&pageNumber=3&rating=4&startDate=12/24/2000&endDate=12/24/2023")]
    [InlineData("orderBy=rating desc")]
    [InlineData("")]
    public async Task GetReviews_WithValidParameters_ReturnsOkResult(string queryParams) {
        var review = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{review.ProductId}/reviews/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageSize=60&pageNumber=30")]
    [InlineData("rating=8")]
    [InlineData("startDate=12/24/2023&endDate=12/24/2000")]
    [InlineData("orderBy=rating,reviewDate desc")]
    [InlineData("orderBy=id desc")]
    public async Task GetReviews_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams) {
        var review = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{review.ProductId}/reviews/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateReview_WithValidModel_ReturnsNoContent() {
        var review = Seed(1, true).First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();
        updateReviewDto.Rating = 4;

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdateReview_WithInvalidModel_ReturnsBadRequest() {
        var review = Seed(1, true).First();
        UpdateReviewDto updateReviewDto = null;

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var review = Seed(1, true).First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();
        updateReviewDto.Rating = 44;

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidProductId_ReturnsNotFound() {
        var review = Seed(1, true).First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}/reviews/{review.Id}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidReviewId_ReturnsNotFound() {
        var review = Seed(1, true).First();
        var updateReviewDto = review.Adapt<UpdateReviewDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{review.ProductId}/reviews/{Guid.NewGuid()}", updateReviewDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteReview_WithValidId_ReturnsNoContent() {
        var review = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{review.ProductId}/reviews/{review.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteReview_WithInvalidProductId_ReturnsNotFound() {
        var review = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/reviews/{review.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteReview_WithInvalidReviewId_ReturnsNotFound() {
        var review = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{review.ProductId}/reviews/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}