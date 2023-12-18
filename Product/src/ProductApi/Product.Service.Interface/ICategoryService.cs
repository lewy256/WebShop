using OneOf;
using OneOf.Types;
using ProductApi.Model.Entities;
using ProductApi.Model.Responses;

namespace ProductApi.Interfaces;
public interface ICategoryService {
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId);
    Task<CategoryCreateResponse> CreateCategoryAsync(Category category);
    Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, Category category);
    Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId);
}


[GenerateOneOf]
public partial class CategoryUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class CategoryCreateResponse : OneOfBase<Category, ValidationFailed> {
}

[GenerateOneOf]
public partial class CategoryDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class CategoryGetResponse : OneOfBase<Category, NotFoundResponse> {
}

