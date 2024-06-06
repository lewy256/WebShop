using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model.PriceHistoryDtos;
using System.Text.Json;

namespace ProductApi.Controllers.V1;

[Route("api/products/{productId}/prices-history")]
[Authorize(Roles = "Administrator")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class PriceHistoryController : ControllerBase {
    private readonly IPriceHistoryService _priceHistoryService;

    public PriceHistoryController(IPriceHistoryService priceHistoryService) {
        _priceHistoryService = priceHistoryService;
    }

    /// <summary>
    /// Retrieves the list of the entire price history.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/products/d7840956-951b-4079-9e99-c09c1726d5d2/prices-history
    /// </remarks>
    /// <param name="productId">The ID of the product for prices to retrieve.</param>
    /// <param name="priceHistoryParameters">The container holds specific parameters for the price history.</param>
    /// <returns>A list of prices.</returns>
    /// <response code="200">Returns the list of prices.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    /// <response code="422">If the price history parameters validation fails.</response>
    [HttpGet(Name = nameof(GetPricesHistoryForProduct))]
    [HttpHead]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PriceHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetPricesHistoryForProduct(Guid productId, [FromQuery] PriceHistoryParameters priceHistoryParameters) {
        var results = await _priceHistoryService.GetPricesHistoryAsync(productId, priceHistoryParameters);

        return results.Match<IActionResult>(
          result => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData,
                  new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
              return Ok(result.pricesHistory);
          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Retrieves the list of the entire price history for a product based on its product ID.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/products/d7840956-951b-4079-9e99-c09c1726d5d2/prices-history/fd28518a-cfad-48d3-85fb-1119bfbbba31
    /// </remarks>
    /// <param name="productId">The ID of the product for pirce history to retrieve.</param>
    /// <param name="priceHistoryId">The ID of the category price history.</param>
    /// <returns>The requested price history.</returns>
    /// <response code="200">Returns the requested price history.</response>
    /// <response code="404">If the price history or product with the given ID is not found.</response>
    [HttpGet("{priceHistoryId:guid}", Name = nameof(GetPriceHistoryForProduct))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PriceHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPriceHistoryForProduct(Guid productId, Guid priceHistoryId) {
        var results = await _priceHistoryService.GetPriceHistoryByIdAsync(productId, priceHistoryId);

        return results.Match<IActionResult>(
          priceHistory => Ok(priceHistory),
          notFound => NotFound(notFound));

    }

    /// <summary>
    /// Creates a new price.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/products/d7840956-951b-4079-9e99-c09c1726d5d2/prices-history
    ///     {        
    ///       "startDate": "11/20/2023",
    ///       "endDate": "12/20/2023",
    ///       "priceValue": "44.44"        
    ///     }
    /// </remarks>
    /// <param name="productId">The ID of the product for price history.</param>
    /// <param name="priceHistory">The created price hsitory information</param>
    /// <returns>A newly created pprice</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the updated price is null.</response>
    /// <response code="422">f the model is invalid or the price history information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPost(Name = nameof(CreatePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreatePriceHistoryForProduct(Guid productId, [FromBody] CreatePriceHistoryDto priceHistory) {
        var results = await _priceHistoryService.CreatePriceHistoryAsync(productId, priceHistory);

        return results.Match<IActionResult>(
            priceHistory => CreatedAtRoute(nameof(GetPriceHistoryForProduct), new { productId, priceHistoryId = priceHistory.Id }, priceHistory),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Updates a specific price by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/products/d7840956-951b-4079-9e99-c09c1726d5d2/prices-history
    ///     {        
    ///       "startDate": "11/20/2023",
    ///       "endDate": "12/20/2023",
    ///       "priceValue": "44.44"        
    ///     }
    /// </remarks>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="priceHistoryId">The ID of the price history to update.</param>
    /// <param name="priceHistory">The updated price history information.</param>
    /// <returns>The updated product.</returns>
    /// <response code="204">If the price history is successfully updated.</response>
    /// <response code="400">If the updated price history is null.</response>
    /// <response code="404">If the product or price history with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the price history information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPut("{priceHistoryId:guid}", Name = nameof(UpdatePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdatePriceHistoryForProduct(Guid productId, Guid priceHistoryId, [FromBody] UpdatePriceHistoryDto priceHistory) {
        var results = await _priceHistoryService.UpdatePriceHistoryAsync(productId, priceHistoryId, priceHistory);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Deletes a specific price by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/products/d7840956-951b-4079-9e99-c09c1726d5d2/prices-history/844288e4-630d-42e7-9667-243bc655569f
    /// </remarks>
    /// <param name="productId">The ID of the product for prices.</param>
    /// <param name="priceHistoryId">The ID of the price history to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the price history is successfully deleted.</response>
    /// <response code="404">If the price history or product with the given ID is not found.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpDelete("{priceHistoryId:guid}", Name = nameof(DeletePriceHistoryForProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePriceHistoryForProduct(Guid productId, Guid priceHistoryId) {
        var results = await _priceHistoryService.DeletePriceHistoryAsync(productId, priceHistoryId);

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
    public IActionResult GetPriceHistoryOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}