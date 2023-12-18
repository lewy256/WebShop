using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.Responses;

namespace ProductApi.Service;

public class CategoryService : ICategoryService {
    private readonly ProductContext _productContext;
    private readonly IValidator<Category> _categoryValidator;

    public CategoryService(ProductContext productContext, IValidator<Category> categoryValidator) {
        _productContext = productContext;
        _categoryValidator = categoryValidator;
    }

    public async Task<CategoryCreateResponse> CreateCategoryAsync(Category category) {
        var validationResult = await _categoryValidator.ValidateAsync(category);

        if(!validationResult.IsValid) {
            return new ValidationFailed(validationResult.Errors);
        }

        await _productContext.Category.AddAsync(category);
        await _productContext.SaveChangesAsync();

        return category;
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

    public async Task<IEnumerable<Category>> GetCategoriesAsync() {
        var categories = await _productContext.Category.ToListAsync();

        return categories;
    }

    public async Task<CategoryGetResponse> GetCategoryByIdAsync(Guid categoryId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(categoryId)); ;

        return category is null ? new NotFoundResponse(categoryId, nameof(category)) : category;
    }

    public async Task<CategoryUpdateResponse> UpdateCategoryAsync(Guid categoryId, Category category) {
        var validationResult = await _categoryValidator.ValidateAsync(category);

        if(!validationResult.IsValid) {
            return new ValidationFailed(validationResult.Errors);

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