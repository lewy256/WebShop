using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.ProductDtos.V1;
using System.Text.Json;

namespace ProductApi.Controllers.V1;

[Route("api/categories/{categoryId}/products")]
[Authorize(Roles = "Administrator")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;

    public ProductController(IProductService productService) {
        _productService = productService;
    }

    /// <summary>
    /// Gets the list of all products by category.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/categories/d7840956-951b-4079-9e99-c09c1726d5d2/products
    /// </remarks>
    /// <param name="categoryId">The ID of the category for products to retrieve.</param>
    /// <param name="productParameters">The container holds specific parameters for the product.</param>
    /// <returns>A list of products.</returns>
    /// <response code="200">Returns the list of products.</response>
    /// <response code="404">If the category with the given ID is not found.</response>
    /// <response code="422">If the product parameters validation fails</response>
    [HttpGet(Name = nameof(GetProductsForCategory))]
    [HttpHead]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetProductsForCategory(Guid categoryId, [FromQuery] ProductParameters productParameters) {
        var results = await _productService.GetProductsAsync(categoryId, productParameters);

        return results.Match<IActionResult>(
          result => {
              Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
              return Ok(result.products);

          },
          notFound => NotFound(notFound),
          validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/categories/d7840956-951b-4079-9e99-c09c1726d5d2/products/fd28518a-cfad-48d3-85fb-1119bfbbba31
    /// </remarks>
    /// <param name="categoryId">The ID of the category for product to retrieve.</param>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <returns>The requested product.</returns>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the cateogry or product with the given ID is not found.</response>
    [HttpGet("{productId:guid}", Name = nameof(GetProductForCategory))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.GetProductByIdAsync(categoryId, productId);

        return results.Match<IActionResult>(
            product => Ok(product),
            notFound => NotFound(notFound));
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/categories/d7840956-951b-4079-9e99-c09c1726d5d2/products
    ///     {        
    ///       "productName": "Andrew",
    ///       "serialNumber": "123456",
    ///       "price": "44.44",
    ///       "stock": "44",
    ///       "description": "Lorem ipsum dolor sit amet, consectetur adipiscing",
    ///       "color": "Pink",
    ///       "weight": "44",
    ///       "size": "4"
    ///     }
    /// </remarks>
    /// <param name="categoryId">The ID of the category for product.</param>
    /// <param name="product">The created rproduct information</param>
    /// <returns>A newly created product.</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the updated product is null.</response>
    /// <response code="422">If the model is invalid or the product information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPost(Name = nameof(CreateProductForCategory))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, [FromBody] CreateProductDto product) {
        var results = await _productService.CreateProductAsync(categoryId, product);

        return results.Match<IActionResult>(
            product => CreatedAtRoute(nameof(GetProductForCategory), new { categoryId, productId = product.Id }, product),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Updates a specific product by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/categories/d7840956-951b-4079-9e99-c09c1726d5d2/products/844288e4-630d-42e7-9667-243bc655569f
    ///     {        
    ///       "productName": "Andrew",
    ///       "serialNumber": "123456",
    ///       "price": "44.44",
    ///       "stock": "44",
    ///       "description": "Lorem ipsum dolor sit amet, consectetur adipiscing",
    ///       "color": "Pink",
    ///       "weight": "44",
    ///       "size": "4"
    ///     }
    /// </remarks>
    /// <param name="categoryId">The ID of the category for product.</param>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="product">The updated product information.</param>
    /// <returns>The updated product.</returns>
    /// <response code="204">If the product is successfully updated.</response>
    /// <response code="400">If the updated product is null.</response>
    /// <response code="404">If the cateogry or product with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the product information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPut("{productId:guid}", Name = nameof(UpdateProductForCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, [FromBody] UpdateProductDto product) {
        var results = await _productService.UpdateProductAsync(categoryId, productId, product);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => NotFound(notFound),
            validationFailed => UnprocessableEntity(validationFailed));
    }

    /// <summary>
    /// Deletes a specific product by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/categories/d7840956-951b-4079-9e99-c09c1726d5d2/products/844288e4-630d-42e7-9667-243bc655569f
    /// </remarks>
    /// <param name="categoryId">The ID of the category for product.</param>
    /// <param name="productId">The ID of the product to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the product is successfully deleted.</response>
    /// <response code="404">If the product or category with the given ID is not found.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpDelete("{productId:guid}", Name = nameof(DeleteProductForCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId) {
        var results = await _productService.DeleteProductAsync(categoryId, productId);

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
    public IActionResult GetProductOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}