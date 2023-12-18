using Microsoft.AspNetCore.Mvc;
using OrderApi.ActionFilters;
using ProductApi.Extensions;
using ProductApi.Interfaces;
using ProductApi.Model.Entities;


namespace ProductApi.Controllers;

//[EnableRateLimiting("SpecificPolicy")]
[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase {
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService) {
        _categoryService = categoryService;
    }



    /// <summary>
    /// Gets the list of all products by category.
    /// </summary>
    /// <param name="categoryId">The ID of the category for products to retrieve.</param>
    /// <returns>A list of products.</returns>
    /// <response code="200">Returns the list of products.</response>
    [HttpGet(Name = "GetCategories")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories() {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }



    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>The requested product.</returns>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    [HttpGet("{categoryId:guid}", Name = "CategoryById")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCateogry(Guid categoryId) {
        var results = await _categoryService.GetCategoryByIdAsync(categoryId);

        return results.Match<IActionResult>(
            category => Ok(category),
            notFound => NotFound(notFound.MapToResponse()));
    }



    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="category"></param>
    /// <returns>A newly created category</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    [HttpPost(Name = "CreateCategory")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCategory([FromBody] Category category) {
        var results = await _categoryService.CreateCategoryAsync(category);

        return results.Match<IActionResult>(
            category => CreatedAtRoute("CategoryById", new { categoryId = category.Id }, category),
            validationFailed => BadRequest(validationFailed.Errors.MapToResponse()));
    }



    /// <summary>
    /// Updates a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="product">The updated product information.</param>
    /// <returns>The updated product.</returns>
    /// <response code="204">If the product is successfully updated.</response>
    /// <response code="400">If the updated product is null or the ID in the URL does not match the ID in the payload.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the product information is incomplete.</response>
    [HttpPut("{categoryId:guid}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] Category category) {
        var results = await _categoryService.UpdateCategoryAsync(categoryId, category);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => NotFound(notFound.MapToResponse()),
            validationFailed => BadRequest(validationFailed.Errors.MapToResponse()));
    }



    /// <summary>
    /// Deletes a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the product is successfully deleted.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    [HttpDelete("{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId) {
        var results = await _categoryService.DeleteCategoryAsync(categoryId);

        return results.Match<IActionResult>(
           _ => NoContent(),
           notFound => NotFound(notFound.MapToResponse()));
    }

    [HttpOptions]
    public IActionResult GetCompaniesOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}

