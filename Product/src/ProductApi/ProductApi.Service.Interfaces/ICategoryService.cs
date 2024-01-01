using OneOf;
using OneOf.Types;
using ProductApi.Model.LinkModels.Categories;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model.CategoryDtos;

namespace ProductApi.Service.Interfaces;
public interface ICategoryService {
    Task<CategoryGetAllResponse> GetCategoriesAsync(LinkCategoryParameters linkCategoryParameters);
    Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId);
    Task<CategoryCreateResponse> CreateCategoryAsync(CreateCategoryDto category);
    Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto category);
    Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId);
}


[GenerateOneOf]
public partial class CategoryUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class CategoryCreateResponse : OneOfBase<CategoryDto, ValidationFailed> {
}

[GenerateOneOf]
public partial class CategoryDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class CategoryGetResponse : OneOfBase<CategoryDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class CategoryGetAllResponse : OneOfBase<CategoryLinkResponse> {
}

