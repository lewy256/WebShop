using Microsoft.EntityFrameworkCore;
using ProductApi.Entities;
using ProductApi.Infrastructure;

namespace ProductApi.Services.V2;

public class CategoryService {
    private readonly ProductContext _productContext;

    public CategoryService(ProductContext productContext) {
        _productContext = productContext;
    }
    public async IAsyncEnumerable<Category> GetCategoriesAsync() {
        // var client=context.Database.GetCosmosClient();
        await foreach(var category in _productContext.Category.AsNoTracking().AsAsyncEnumerable()) {
            yield return category;
        }
    }

}