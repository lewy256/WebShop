using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.Filters;
using ProductApi.Services;
using ProductApi.Shared.LinkModels.PriceHistory;
using ProductApi.Shared.PriceHistoryDtos;
using System.Text.Json;

namespace ProductApi.Controllers;

[Route("api/products/{productId}/price-histories")]
[Authorize(Policy = "RequireAdministratorRole")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class PriceHistoryController : BaseController {
    private readonly IPriceHistoryService _priceHistoryService;

    public PriceHistoryController(IPriceHistoryService priceHistoryService) {
        _priceHistoryService = priceHistoryService;
    }

    [HttpGet(Name = nameof(GetPriceHistoriesForProduct))]
    [HttpHead]
    [AllowAnonymous]
    [Produces("application/json", "application/vnd.lewy256.hateoas+json")]
    [ProducesResponseType(typeof(IEnumerable<PriceHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetPriceHistoriesForProduct(Guid productId, [FromQuery] PriceHistoryParameters priceHistoryParameters) {
        var linkParams = new LinkPriceHistoryParameters(priceHistoryParameters, HttpContext);
        var results = await _priceHistoryService.GetPriceHistoriesAsync(productId, linkParams);

        return results.Match<IActionResult>(
          response => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.metaData));
              return response.linkResponse.HasLinks ? Ok(response.linkResponse.LinkedEntity) : Ok(response.linkResponse.PricesHistoryDto);
          },
          notFound => Problem(notFound),
          validationFailed => Problem(validationFailed));
    }

    [HttpGet("{priceHistoryId:guid}", Name = nameof(GetPriceHistoryForProduct))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PriceHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPriceHistoryForProduct(Guid productId, Guid priceHistoryId) {
        var results = await _priceHistoryService.GetPriceHistoryByIdAsync(productId, priceHistoryId);

        return results.Match<IActionResult>(
          priceHistory => Ok(priceHistory),
          notFound => Problem(notFound));

    }

    [HttpPost(Name = nameof(CreatePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreatePriceHistoryForProduct(Guid productId, [FromBody] CreatePriceHistoryDto priceHistory) {
        var results = await _priceHistoryService.CreatePriceHistoryAsync(productId, priceHistory);

        return results.Match<IActionResult>(
            priceHistory => CreatedAtRoute(nameof(GetPriceHistoryForProduct), new { productId, priceHistoryId = priceHistory.Id }, priceHistory),
            notFound => Problem(notFound),
            validationFailed => Problem(validationFailed));
    }


    [HttpPut("{priceHistoryId:guid}", Name = nameof(UpdatePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdatePriceHistoryForProduct(Guid productId, Guid priceHistoryId, [FromBody] UpdatePriceHistoryDto priceHistory) {
        var results = await _priceHistoryService.UpdatePriceHistoryAsync(productId, priceHistoryId, priceHistory);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => Problem(notFound),
            validationFailed => Problem(validationFailed));
    }


    [HttpDelete("{priceHistoryId:guid}", Name = nameof(DeletePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletePriceHistoryForProduct(Guid productId, Guid priceHistoryId) {
        var results = await _priceHistoryService.DeletePriceHistoryAsync(productId, priceHistoryId);

        return results.Match<IActionResult>(
          _ => NoContent(),
          notFound => Problem(notFound));
    }

    [HttpOptions]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPriceHistoryOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}