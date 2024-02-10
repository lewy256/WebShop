using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Model.LinkModels.Products;
using ProductApi.Service.V2;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.ProductDtos.V2;
using System.Text.Json;

namespace ProductApi.Controllers.V2;

[Route("api/categories/{categoryId}/products")]
[ApiExplorerSettings(GroupName = "v2")]
[ApiController]
public class ProductController : ControllerBase {
    private readonly ProductService _productService;

    public ProductController(ProductService productService) {
        _productService = productService;
    }

    [HttpGet(Name = nameof(GetProductsForCategory))]
    [HttpHead]
    [AllowAnonymous]
    [Produces("application/json", "application/vnd.lewy256.hateoas+json")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetProductsForCategory(Guid categoryId, [FromQuery] ProductParameters productParameters) {
        var linkParams = new LinkProductParameters(productParameters, HttpContext);
        var results = await _productService.GetProductsAsync(categoryId, linkParams);

        return results.Match<IActionResult>(
          response => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.metaData));

              if(!response.linkResponse.IsShaped) {
                  return Ok(response.linkResponse.Products);
              }
              else if(response.linkResponse.HasLinks) {
                  return Ok(response.linkResponse.LinkedEntities);
              }

              return Ok(response.linkResponse.ShapedEntities);

          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }
}
