using Microsoft.AspNetCore.Mvc;
using ProductApi.ActionFilters;
using ProductApi.Interfaces;
using ProductApi.Shared.Model;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;
    private readonly IProductLinks _productLinks;

    public ProductController(IProductService productService, IProductLinks productLinks) {
        _productService = productService;
        _productLinks = productLinks;
    }




    /// <summary>
    /// Gets the list of all products by category
    /// </summary>
    /// <returns>The products list</returns>
 /*   [HttpGet("{categoryId:Guid}")]
    public async Task<IActionResult> GetProducts(Guid categoryId, [FromQuery] ProductParameters productParameters) {
        var pagedResult = await _productService.GetProductsAsync(categoryId, productParameters);


        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.productsDto);
    }*/



    [HttpGet("{productId:guid}", Name = "ProductById")]
    public async Task<IActionResult> GetProduct(Guid productId) {
        var product = await _productService.GetProductByIdAsync(productId);
        return Ok(product);
    }




    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>A newly created company</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product) {
        var createdProduct = await _productService.CreateProductAsync(product);
        return CreatedAtRoute("ProductById", new { productId = createdProduct.Id },
            createdProduct);
    }

    [HttpPut("{productId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> PutProduct(Guid productId, [FromBody] UpdateProductDto product) {
        await _productService.UpdateProductAsync(productId, product);

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId) {
        await _productService.DeleteProductAsync(productId);

        return NoContent();
    }
}