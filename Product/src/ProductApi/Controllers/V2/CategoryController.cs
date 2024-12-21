using Microsoft.AspNetCore.Mvc;
using ProductApi.Entities;
using ProductApi.Services.V2;

namespace ProductApi.Controllers.V2;

[Route("api/categories")]
[ApiExplorerSettings(GroupName = "v2")]
[ApiController]
public class CategoryController : ControllerBase {
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService) {
        _categoryService = categoryService;
    }

    [HttpGet(Name = nameof(GetCategories))]
    public async IAsyncEnumerable<Category> GetCategories() {
        await foreach(var category in _categoryService.GetCategoriesAsync()) {
            yield return category;
        }
    }
}
