using Microsoft.AspNetCore.Mvc;
using ProductApi.ActionFilters;
using ProductApi.Interfaces;
using ProductApi.Shared.Model;
using System.Text.Json;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;


    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{categoryId:int}")]
    public async Task<IActionResult> GetProducts(int categoryId, [FromQuery] ProductParameters productParameters)
    {
        var pagedResult = await _productService.GetProductsAsync(categoryId, productParameters);


        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.productsDto);
    }

    [HttpGet("{productId:guid}", Name = "ProductById")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductParameters productParameters)
    {
        var pagedResult = await _productService.GetProductsAsync(productParameters);

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.productsDto);
    }


    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
    {
        var createdProduct = await _productService.CreateProductAsync(product);
        return CreatedAtRoute("ProductById", new { productId = createdProduct.Id },
            createdProduct);
    }

    [HttpPut("{productId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> PutProduct(Guid productId, [FromBody] UpdateProductDto product)
    {
        await _productService.UpdateProductAsync(productId, product);

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        await _productService.DeleteProductAsync(productId);

        return NoContent();
    }
}