using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Categories;
using ProductApi.Service.V2;

namespace ProductApi.Controllers.V2;

[Route("api/categories")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class CategoryController : ControllerBase {
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService) {
        _categoryService = categoryService;
    }

    [HttpGet(Name = nameof(GetCategories))]
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
}
