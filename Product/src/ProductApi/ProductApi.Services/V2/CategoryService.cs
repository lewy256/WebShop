using Microsoft.EntityFrameworkCore;
using OneOf;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.LinkModels.Categories;

namespace ProductApi.Service.V2;

public class CategoryService {
    private readonly ProductContext _productContext;
    private readonly ICategoryLinks _categoryLinks;

    public CategoryService(ProductContext productContext, ICategoryLinks categoryLinks) {
        _productContext = productContext;
        _categoryLinks = categoryLinks;
    }

    public async Task<CategoryGetAllResponse> GetCategoriesAsync(LinkCategoryParameters linkCategoryParameters) {
        var categories = await _productContext.Category.AsNoTracking().ToListAsync();

        var links = _categoryLinks.TryGenerateLinks(categories, linkCategoryParameters.Context);

        return links;
    }
}

[GenerateOneOf]
public partial class CategoryGetAllResponse : OneOfBase<CategoryLinkResponse> {
}
