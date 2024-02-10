using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Service.V2;
using ProductApi.Shared.Model.PriceHistoryDtos;
using System.Text.Json;

namespace ProductApi.Controllers.V2;

[ApiController]
[Route("api/products/{productId}/prices-history")]
[ApiExplorerSettings(GroupName = "v2")]
public class PriceHistoryController : ControllerBase {
    private readonly PriceHistoryService _priceHistoryService;

    public PriceHistoryController(PriceHistoryService priceHistoryService) {
        _priceHistoryService = priceHistoryService;
    }

    [HttpGet(Name = nameof(GetPricesHistoryForProduct))]
    [HttpHead]
    [AllowAnonymous]
    [Produces("application/json", "application/vnd.lewy256.hateoas+json")]
    [ProducesResponseType(typeof(IEnumerable<PriceHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetPricesHistoryForProduct(Guid productId, [FromQuery] PriceHistoryParameters priceHistoryParameters) {
        var linkParams = new LinkPriceHistoryParameters(priceHistoryParameters, HttpContext);
        var results = await _priceHistoryService.GetPricesHistoryAsync(productId, linkParams);

        return results.Match<IActionResult>(
          response => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.metaData));
              return response.linkResponse.HasLinks ? Ok(response.linkResponse.LinkedEntity) : Ok(response.linkResponse.PricesHistoryDto);
          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }
}
