using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Service.V2;
using ProductApi.Shared.Model.ReviewDtos;
using System.Text.Json;

namespace ProductApi.Controllers.V2;

[ApiController]
[Route("api/products/{productId}/reviews")]
[ApiExplorerSettings(GroupName = "v2")]
public class ReviewController : ControllerBase {
    private readonly ReviewService _reviewService;

    public ReviewController(ReviewService reviewService) {
        _reviewService = reviewService;
    }

    [HttpGet(Name = nameof(GetReviewsForProduct))]
    [HttpHead]
    [AllowAnonymous]
    [Produces("application/json", "application/vnd.lewy256.hateoas+json")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetReviewsForProduct(Guid productId, [FromQuery] ReviewParameters reviewParameters) {
        var linkParams = new LinkReviewParameters(reviewParameters, HttpContext);
        var results = await _reviewService.GetReviewsAsync(productId, linkParams);

        return results.Match<IActionResult>(
          response => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.metaData));
              return response.linkResponse.HasLinks ? Ok(response.linkResponse.LinkedEntity) : Ok(response.linkResponse.Reviews);
          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }
}
