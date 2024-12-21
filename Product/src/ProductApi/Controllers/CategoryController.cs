using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Entities;
using ProductApi.Infrastructure.Filters;
using ProductApi.Services;
using ProductApi.Shared.CategoryDtos;
using ProductApi.Shared.LinkModels.Categories;

namespace ProductApi.Controllers;

[Route("api/categories")]
[Authorize(Policy = "RequireAdministratorRole")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class CategoryController : BaseController {
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService) {
        _categoryService = categoryService;
    }


    /// <summary>
    /// Gets the list of all categories.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/categories
    /// </remarks>
    /// <returns>A list of categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    [HttpGet(Name = nameof(GetCategories))]
    [AllowAnonymous]
    [Produces("application/json", "application/vnd.lewy256.hateoas+json")]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetCategories() {
        var linkParams = new LinkCategoryParameters(HttpContext);
        var results = await _categoryService.GetCategoriesAsync(linkParams);

        return results.Match<IActionResult>(
            response => {
                return response.HasLinks ? Ok(response.LinkedEntity) : Ok(response.Categories);
            });
    }


    /// <summary>
    /// Retrieves a specific category by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/categories/8bc454b9-a196-431d-8980-d41df332dc70
    /// </remarks>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>The requested category.</returns>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category with the given ID is not found.</response>
    [HttpGet("{categoryId:guid}", Name = nameof(GetCategory))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(Guid categoryId) {
        var results = await _categoryService.GetCategoryByIdAsync(categoryId);


        return results.Match<IActionResult>(
            category => Ok(category),
            notFound => Problem(notFound));
    }


    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/categories
    ///     {        
    ///         "categoryName ": "rockets"   
    ///     }
    /// </remarks>
    /// <param name="category"></param>
    /// <returns>A newly created category</returns>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the item is null.</response>
    /// <response code="422">If the model is invalid.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPost(Name = nameof(CreateCategory))]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto category) {
        var results = await _categoryService.CreateCategoryAsync(category);

        return results.Match<IActionResult>(
            category => CreatedAtRoute(nameof(GetCategory), new { categoryId = category.Id }, category),
            validationFailed => Problem(validationFailed));
    }


    /// <summary>
    /// Updates a specific category by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/categories/8bc454b9-a196-431d-8980-d41df332dc70
    ///     {        
    ///       "categoryName ": "rockets"         
    ///     }
    /// </remarks>
    /// <param name="categoryId">The ID of the category to update.</param>
    /// <param name="category">The updated category information.</param>
    /// <returns>The updated category.</returns>
    /// <response code="204">If the category is successfully updated.</response>
    /// <response code="400">If the updated category is null.</response>
    /// <response code="404">If the category with the given ID is not found.</response>
    /// <response code="422">If the model is invalid or the category information is incomplete.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPut("{categoryId:guid}", Name = nameof(UpdateCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto category) {
        var results = await _categoryService.UpdateCategoryAsync(categoryId, category);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => Problem(notFound),
            validationFailed => Problem(validationFailed));
    }


    /// <summary>
    /// Deletes a specific category by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/categories/8bc454b9-a196-431d-8980-d41df332dc70
    /// </remarks>
    /// <param name="categoryId">The ID of the category to delete.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the category is successfully deleted.</response>
    /// <response code="404">If the category with the given ID is not found.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpDelete("{categoryId:guid}", Name = nameof(DeleteCategory))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId) {
        var results = await _categoryService.DeleteCategoryAsync(categoryId);

        return results.Match<IActionResult>(
           _ => NoContent(),
           notFound => Problem(notFound));
    }

    /// <summary>
    /// Returns an Allow header containing the allowable HTTP methods.
    /// </summary>
    [HttpOptions]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategoryOptions() {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}

