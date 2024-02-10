using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.ReviewDtos;
using System.Text.Json;

namespace ProductApi.Controllers.V1;

[Route("api/products/{productId}/reviews")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class ReviewController : ControllerBase {
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService) {
        _reviewService = reviewService;
    }


    /// <summary>
    /// Gets the list of all reviews by product.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/products/d7840956-951b-4079-9e99-c09c1726d5d2/reviews
    /// </remarks>
    /// <param name="productId">The ID of the product for reviews to retrieve.</param>
    /// <param name="reviewParameters">The container holds specific parameters for the review.</param>
    /// <returns>A list of reviews.</returns>
    /// <response code="200">Returns the list of reviews.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    /// <response code="422">If the review parameters validation fails.</response>
    [HttpGet(Name = nameof(GetReviewsForProduct))]
    [HttpHead]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetReviewsForProduct(Guid productId, [FromQuery] ReviewParameters reviewParameters) {
        var results = await _reviewService.GetReviewsAsync(productId, reviewParameters);

        return results.Match<IActionResult>(
          result => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
              return Ok(result.reviews);
          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Retrieves a specific review by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/products/d7840956-951b-4079-9e99-c09c1726d5d2/reviews/844288e4-630d-42e7-9667-243bc655569f
    /// </remarks>
    /// <param name="productId">The ID of the product for review to retrieve.</param>
    /// <param name="reviewId">The ID of the review.</param>
    /// <returns>The requested review.</returns>
    /// <response code="200">Returns the requested review.</response>
    /// <response code="404">If the review or product with the given ID is not found.</response>
    [HttpGet("{reviewId:guid}", Name = nameof(GetReviewForProduct))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewForProduct(Guid productId, Guid reviewId) {
        var results = await _reviewService.GetReviewByIdAsync(productId, reviewId);

        return results.Match<IActionResult>(
            review => Ok(review),
            notFound => NotFound(notFound));

    }

    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/products/d7840956-951b-4079-9e99-c09c1726d5d2/reviews/844288e4-630d-42e7-9667-243bc655569f
    ///     {        
    ///       "description": "Lorem ipsum dolor sit amet, consectetur adipiscing",
    ///       "rating": "4"
    ///     }
    /// </remarks>
    /// <param name="productId">The ID of the product for review.</param>
    /// <param name="review">The created review information</param>
    /// <returns>A newly created review.</returns>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the updated review is null.</response>
    /// <response code="422">If the model is invalid or the review information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPost(Name = nameof(CreateReviewForProduct)), Authorize(Roles = "Administrator, Customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateReviewForProduct(Guid productId, [FromBody] CreateReviewDto review) {
        var results = await _reviewService.CreateReviewAsync(productId, review);

        return results.Match<IActionResult>(
            review => CreatedAtRoute(nameof(GetReviewForProduct), new { productId, reviewId = review.Id }, review),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Updates a specific review by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/products/d7840956-951b-4079-9e99-c09c1726d5d2/reviews
    ///     {        
    ///       "description": "Lorem ipsum dolor sit amet, consectetur adipiscing",
    ///       "rating": "4"  
    ///     }
    /// </remarks>
    /// <param name="productId">The ID of the product for review.</param>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="review">The updated review information.</param>
    /// <returns>The updated review.</returns>
    /// <response code="204">If the review is successfully updated.</response>
    /// <response code="400">If the updated review is null.</response>
    /// <response code="404">If the review or product with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the review information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPut("{reviewId:guid}", Name = nameof(UpdateReviewForProduct)), Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateReviewForProduct(Guid productId, Guid reviewId, [FromBody] UpdateReviewDto review) {
        var results = await _reviewService.UpdateReviewAsync(productId, reviewId, review);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Deletes a specific review by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/products/d7840956-951b-4079-9e99-c09c1726d5d2/reviews/844288e4-630d-42e7-9667-243bc655569f
    /// </remarks>
    /// <param name="productId">The ID of the product for reviews.</param>
    /// <param name="reviewId">The ID of the product to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the review is successfully deleted.</response>
    /// <response code="404">If the review or product with the given ID is not found.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpDelete("{reviewId:guid}", Name = nameof(DeleteReviewForProduct)), Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteReviewForProduct(Guid productId, Guid reviewId) {
        var results = await _reviewService.DeleteReviewAsync(productId, reviewId);

        return results.Match<IActionResult>(
          _ => NoContent(),
          notFound => NotFound(notFound));
    }

    /// <summary>
    /// Returns an Allow header containing the allowable HTTP methods.
    /// </summary>
    [HttpOptions]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReviewOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}