using OneOf;
using OneOf.Types;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.CategoryDtos;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.Interfaces;
public interface ICategoryService {
    Task<CategoriesGetAllResponse> GetCategoriesAsync();
    Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId);
    Task<CategoryCreateResponse> CreateCategoryAsync(CreateCategoryDto category);
    Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto category);
    Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId);
}


[GenerateOneOf]
public partial class CategoryUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class CategoryCreateResponse : OneOfBase<Category, ValidationResponse> {
}

[GenerateOneOf]
public partial class CategoryDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class CategoryGetResponse : OneOfBase<Category, NotFoundResponse> {
}
public record CategoriesGetAllResponse(IEnumerable<Category> Categories);
