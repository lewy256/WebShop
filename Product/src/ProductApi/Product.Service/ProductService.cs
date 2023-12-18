using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Service;

public class ProductService : IProductService {
    private readonly ProductContext _productContext;
    private readonly IProductLinks _productLinks;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly IValidator<ProductParameters> _parametersValidator;

    public ProductService(ProductContext productContext, IProductLinks productLinks,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        IValidator<ProductParameters> parametersValidator) {
        _productContext = productContext;
        _productLinks = productLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
    }


    public async Task<ProductGetAllResponse> GetProductsAsync(Guid categoryId, LinkProductParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.ProductParameters);

        if(!validationResult.IsValid) {
            return new ValidationFailed(validationResult.Errors);
        }

        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        var products = await _productContext.Product
            .Where(p => p.CategoryId.Equals(categoryId) && p.Price >= linkParameters.ProductParameters.MinPrice &&
                        p.Price <= linkParameters.ProductParameters.MaxPrice)
            .Skip((linkParameters.ProductParameters.PageNumber - 1) * linkParameters.ProductParameters.PageSize)
            .Take(linkParameters.ProductParameters.PageSize)
            .ToListAsync();

        var productsDto = products.Adapt<IEnumerable<ProductDto>>();


        var count = await _productContext.Product
            .Where(p => p.CategoryId.Equals(categoryId))
            .CountAsync();

        var links = _productLinks.TryGenerateLinks(productsDto, linkParameters.ProductParameters.Fields, categoryId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ProductParameters.PageNumber,
            PageSize = linkParameters.ProductParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductGetResponse> GetProductByIdAsync(Guid categoryId, Guid productId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var productDto = product.Adapt<ProductDto>();

        return productDto;
    }

    public async Task<ProductCreateResponse> CreateProductAsync(Guid categoryId, CreateProductDto productDto) {
        var validationResult = await _createValidator.ValidateAsync(productDto);

        if(!validationResult.IsValid) {
            return new ValidationFailed(validationResult.Errors);
        }

        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        var product = productDto.Adapt<Product>();

        product.Id = Guid.NewGuid();
        product.CategoryId = category.Id;
        product.CategoryName = category.CategoryName;

        await _productContext.AddAsync(product);
        await _productContext.SaveChangesAsync();

        var productToReturn = product.Adapt<ProductDto>();

        return productToReturn;
    }

    public async Task<ProductUpdateResponse> UpdateProductAsync(Guid categoryId, Guid productId, UpdateProductDto productDto) {
        var validationResult = await _updateValidator.ValidateAsync(productDto);

        if(!validationResult.IsValid) {
            return new ValidationFailed(validationResult.Errors);
        }


        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        var product = await _productContext.Product.SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        productDto.Adapt(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<ProductDeleteResponse> DeleteProductAsync(Guid categoryId, Guid productId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(category));
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        _productContext.Product.Remove(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
}