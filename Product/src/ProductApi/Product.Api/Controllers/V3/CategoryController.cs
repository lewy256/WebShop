using Microsoft.AspNetCore.Mvc;
using ProductApi.Model.Entities;
using ProductApi.Service.V3;

namespace ProductApi.Controllers.V3;

[Route("api/categories")]
[ApiExplorerSettings(GroupName = "v3")]
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
