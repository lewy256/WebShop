using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Categories;
using ProductApi.Model.Responses;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model.CategoryDtos;

namespace ProductApi.Service;

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
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }

        var entity = category.Adapt<Category>();

        entity.Id = Guid.NewGuid();

        await _productContext.Category.AddAsync(entity);
        await _productContext.SaveChangesAsync();

        var categoryToReturn = entity.Adapt<CategoryDto>();

        return categoryToReturn;
    }

    public async Task<CategoryDeleteResponse> DeleteCategoryAsync(Guid categoryId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        _productContext.Category.Remove(category);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<CategoryGetAllResponse> GetCategoriesAsync(LinkCategoryParameters linkCategoryParameters) {
        var categories = await _productContext.Category.AsNoTracking().ToListAsync();

        var links = _categoryLinks.TryGenerateLinks(categories, linkCategoryParameters.Context);

        return links;
    }

    public async Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(categoryId));

        var categoryToReturn = category.Adapt<CategoryDto>();

        return category is null ? new NotFoundResponse(categoryId, nameof(category)) : categoryToReturn;
    }

    public async Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto category) {
        var validationResult = await _updateValidator.ValidateAsync(category);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);

        }

        var categoryEntity = await _productContext.Category.SingleOrDefaultAsync(p => p.Id.Equals(categoryId));

        if(categoryEntity is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        category.Adapt(categoryEntity);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

}