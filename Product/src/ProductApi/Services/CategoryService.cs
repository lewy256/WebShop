using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Utility;
using ProductApi.Shared;
using ProductApi.Shared.CategoryDtos;
using ProductApi.Shared.LinkModels.Categories;
using ProductApi.Shared.Responses;

namespace ProductApi.Services;

public interface ICategoryService {
    Task<CategoryCreateResponse> CreateCategoryAsync(CreateCategoryDto category);
    Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId);
    Task<CategoriesGetAllResponse> GetCategoriesAsync(LinkCategoryParameters linkCategoryParameters);
    Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId);
    Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto category);
}

public class CategoryService : ICategoryService {
    private readonly ProductContext _productContext;
    private readonly IValidator<UpdateCategoryDto> _updateValidator;
    private readonly IValidator<CreateCategoryDto> _createValidator;
    private readonly ICategoryLinks _categoryLinks;

    public CategoryService(ProductContext productContext, IValidator<CreateCategoryDto> createValidator,
        IValidator<UpdateCategoryDto> updateValidator, ICategoryLinks categoryLinks) {
        _productContext = productContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _categoryLinks = categoryLinks;
    }

    public async Task<CategoryCreateResponse> CreateCategoryAsync(CreateCategoryDto category) {
        var validationResult = await _createValidator.ValidateAsync(category);

        if(!validationResult.IsValid) {
            //var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            var vaildationFailed = validationResult.Errors.Select(x => new ValidationError() {
                PropertyName = x.PropertyName,
                ErrorMessage = x.ErrorMessage
            });

            return new ValidationResponse(vaildationFailed);
        }

        var entity = category.Adapt<Category>();

        entity.Id = Guid.NewGuid();

        await _productContext.Category.AddAsync(entity);
        await _productContext.SaveChangesAsync();

        return entity;
    }



    public async Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        _productContext.Category.Remove(category);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<CategoriesGetAllResponse> GetCategoriesAsync(LinkCategoryParameters linkCategoryParameters) {
        var categories = await _productContext.Category.AsNoTracking().ToListAsync();

        var links = _categoryLinks.TryGenerateLinks(categories, linkCategoryParameters.Context);

        return new CategoriesGetAllResponse(links);
    }


    public async Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(categoryId));

        var categoryToReturn = category.Adapt<Category>();

        return category is null ? new NotFoundResponse(categoryId, nameof(Category)) : categoryToReturn;
    }

    public async Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto category) {
        var validationResult = await _updateValidator.ValidateAsync(category);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var categoryEntity = await _productContext.Category.SingleOrDefaultAsync(p => p.Id.Equals(categoryId));

        if(categoryEntity is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        category.Adapt(categoryEntity);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
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

[GenerateOneOf]
public partial class CategoriesGetAllResponse : OneOfBase<CategoryLinkResponse> {

}
