using Microsoft.AspNetCore.Mvc;
using OrderApi.ActionFilters;
using ProductApi.ActionFilters;
using ProductApi.Extensions;
using ProductApi.Interfaces;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels;
using ProductApi.Shared.Model.ProductDtos;
using System.Text.Json;

namespace ProductApi.Controllers;

[Route("api/categories/{categoryId}/products")]
[ApiController]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;

    public ProductController(IProductService productService) {
        _productService = productService;
    }

    /// <summary>
    /// Gets the list of all products by category.
    /// </summary>
    /// <param name="categoryId">The ID of the category for products to retrieve.</param>
    /// <returns>A list of products.</returns>
    /// <response code="200">Returns the list of products.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetProductsForCategory(Guid categoryId, [FromQuery] ProductParameters productParameters) {
        var linkParams = new LinkProductParameters(productParameters, HttpContext);
        var results = await _productService.GetProductsAsync(categoryId, linkParams);

        return results.Match<IActionResult>(
          response => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.metaData));
              return response.linkResponse.HasLinks ? Ok(response.linkResponse.LinkedEntities) : Ok(response.linkResponse.ShapedEntities);
          },
          notFound => NotFound(notFound.MapToResponse()),
          validationFailed => BadRequest(validationFailed.Errors.MapToResponse()));
    }


    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <param name="categoryId">The ID of the category for product to retrieve.</param>
    /// <returns>The requested product.</returns>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    [HttpGet("{productId:guid}", Name = "GetProductForCategory")]
    [HttpHead]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.GetProductByIdAsync(categoryId, productId);

        return results.Match<IActionResult>(
            product => Ok(product),
            notFound => NotFound(notFound.MapToResponse()));
    }


    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>A newly created product</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, [FromBody] CreateProductDto product) {
        var results = await _productService.CreateProductAsync(categoryId, product);

        return results.Match<IActionResult>(
            product => CreatedAtRoute("ProductById", new { productId = product.Id }, product),
            notFound => NotFound(notFound.MapToResponse()),
            validationFailed => BadRequest(validationFailed.Errors.MapToResponse()));
    }

    /// <summary>
    /// Updates a specific product by its unique identifier.
    /// </summary>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="product">The updated product information.</param>
    /// <returns>The updated product.</returns>
    /// <response code="200">Returns the updated product.</response>
    /// <response code="400">If the updated product is null or the ID in the URL does not match the ID in the payload.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the product information is incomplete.</response>
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, [FromBody] UpdateProductDto product) {
        var results = await _productService.UpdateProductAsync(categoryId, productId, product);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => NotFound(notFound.MapToResponse()),
            validationFailed => BadRequest(validationFailed.Errors.MapToResponse()));
    }

    /// <summary>
    /// Deletes a specific product by its unique identifier.
    /// </summary>
    /// <param name="productId">The ID of the product to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the product is successfully deleted.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    //[ApiExplorerSettings(GroupName = "v2")]
    //[ApiVersion("2.0")]
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.DeleteProductAsync(categoryId, productId);

        return results.Match<IActionResult>(
          _ => NoContent(),
          notFound => NotFound(notFound.MapToResponse()));
    }
}