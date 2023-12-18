using Microsoft.AspNetCore.Mvc;
using ProductApi.Interfaces;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Controllers;

[Route("api/products/{productId}/reviews")]
[ApiController]
public class ReviewController : ControllerBase {
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService) {
        _reviewService = reviewService;
    }


    /*    [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetReviewsForProduct(Guid productId, [FromQuery] ReviewParameters reviewParameters) {
            var linkParams = new LinkReviewParameters(reviewParameters, HttpContext);
            var reviews = await _reviewService.GetReviewsAsync(productId, linkParams);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(reviews.metaData));

            return Ok(reviews.linkResponse);
        }*/

    [HttpGet("{reviewId:guid}", Name = "ReviewById")]
    public async Task<IActionResult> GetReviewForProduct(Guid productId, Guid reviewId) {
        var results = await _reviewService.GetReviewByIdAsync(productId, reviewId);

        return Ok(results);

    }


    [HttpPost]
    public async Task<IActionResult> CreateReviewForProduct(Guid productId, [FromBody] CreateReviewDto review) {
        var createdReview = await _reviewService.CreateReviewAsync(productId, review);
        return CreatedAtRoute("ReviewById", new { reviewId = createdReview.Id },
            createdReview);
    }

    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> UpdateReviewForProduct(Guid productId, Guid reviewId, [FromBody] UpdateReviewDto review) {
        await _reviewService.UpdateReviewAsync(productId, reviewId, review);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReviewForProduct(Guid productId, Guid reviewId) {
        await _reviewService.DeleteReviewAsync(productId, reviewId);

        return NoContent();
    }
}