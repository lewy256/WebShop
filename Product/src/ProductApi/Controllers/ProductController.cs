using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.Filters;
using ProductApi.Services;
using ProductApi.Shared.LinkModels.Products;
using ProductApi.Shared.ProductDtos;
using System.Text.Json;

namespace ProductApi.Controllers;

[Route("api/categories/{categoryId}/products")]
[Authorize(Policy = "RequireAdministratorRole")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class ProductController : BaseController {
    private readonly IProductService _productService;

    public ProductController(IProductService productService) {
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
              return response.linkResponse.HasLinks ? Ok(response.linkResponse.LinkedEntities) : Ok(response.linkResponse.Products);

          },
          notFound => Problem(notFound),
          validationFailed => Problem(validationFailed));
    }


    [HttpGet("{productId:guid}", Name = nameof(GetProductForCategory))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.GetProductByIdAsync(categoryId, productId);

        return results.Match<IActionResult>(
            product => Ok(product),
            notFound => Problem(notFound));
    }

    [HttpPost(Name = nameof(CreateProductForCategory))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, [FromBody] CreateProductDto product) {
        var results = await _productService.CreateProductAsync(categoryId, product);

        return results.Match<IActionResult>(
            product => CreatedAtRoute(nameof(GetProductForCategory), new { categoryId, productId = product.Id }, product),
            notFound => Problem(notFound),
            validationFailed => Problem(validationFailed));
    }

    [HttpPut("{productId:guid}", Name = nameof(UpdateProductForCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, [FromBody] UpdateProductDto product) {
        var results = await _productService.UpdateProductAsync(categoryId, productId, product);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => Problem(notFound),
            validationFailed => Problem(validationFailed));
    }


    [HttpDelete("{productId:guid}", Name = nameof(DeleteProductForCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.DeleteProductAsync(categoryId, productId);

        return results.Match<IActionResult>(
          _ => NoContent(),
          notFound => Problem(notFound));
    }

    [HttpOptions]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");
        return Ok();
    }

    [HttpGet("/api/products", Name = nameof(GetAllProducts))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductParameters productParameters) {
        var results = await _productService.GetProductsAsync(productParameters);

        return results.Match<IActionResult>(
          products => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(products.metaData,
                  new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
              return Ok(products.products);
          });
    }
}