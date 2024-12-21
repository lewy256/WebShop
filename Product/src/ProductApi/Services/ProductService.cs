using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Extensions;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Utility;
using ProductApi.Shared;
using ProductApi.Shared.LinkModels.Products;
using ProductApi.Shared.ProductDtos;
using ProductApi.Shared.Responses;

namespace ProductApi.Services;

public interface IProductService {
    Task<ProductsGetAllResponse> GetProductsAsync(Guid categoryId, LinkProductParameters productParameters);
    Task<ProductsGetResponse> GetProductsAsync(ProductParameters productParameters);
    Task<ProductGetResponse> GetProductByIdAsync(Guid categoryId, Guid productId);
    Task<ProductCreateResponse> CreateProductAsync(Guid categoryId, CreateProductDto productDto);
    Task<ProductUpdateResponse> UpdateProductAsync(Guid categoryId, Guid productId, UpdateProductDto productDto);
    Task<ProductDeleteResponse> DeleteProductAsync(Guid categoryId, Guid productId);
}


public class ProductService : IProductService {
    private readonly ProductContext _productContext;
    private readonly IProductLinks _productLinks;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly IValidator<ProductParameters> _parametersValidator;

    public ProductService(ProductContext productContext,
        IProductLinks productLinks,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        IValidator<ProductParameters> parametersValidator
        ) {
        _productContext = productContext;
        _productLinks = productLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
    }

    public async Task<ProductsGetAllResponse> GetProductsAsync(Guid categoryId, LinkProductParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.ProductParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        var query = _productContext.Product
          .AsNoTracking()
          .Where(p => p.CategoryId.Equals(categoryId));

        var products = await query
            .FilterProducts(linkParameters.ProductParameters)
            .SearchProducts(linkParameters.ProductParameters.SearchTerm)
            .SortProducts(linkParameters.ProductParameters.OrderBy)
            .Skip((linkParameters.ProductParameters.PageNumber - 1) * linkParameters.ProductParameters.PageSize)
            .Take(linkParameters.ProductParameters.PageSize)
            .ToListAsync();

        var count = await query.CountAsync();

        var productsDto = products.Adapt<List<ProductDto>>();

        var links = _productLinks.TryGenerateLinks(productsDto, categoryId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ProductParameters.PageNumber,
            PageSize = linkParameters.ProductParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductsGetResponse> GetProductsAsync(ProductParameters productParameters) {
        var query = _productContext.Product.AsNoTracking();

        var products = await query
            .FilterProducts(productParameters)
            .SearchProducts(productParameters.SearchTerm)
            .SortProducts(productParameters.OrderBy)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .ToListAsync();

        var count = await query.CountAsync();

        var productsDto = products.Adapt<List<ProductDto>>();

        return (products: productsDto, metaData: new MetaData() {
            CurrentPage = productParameters.PageNumber,
            PageSize = productParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductGetResponse> GetProductByIdAsync(Guid categoryId, Guid productId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var productDto = product.Adapt<ProductDto>();

        return productDto;
    }

    public async Task<ProductCreateResponse> CreateProductAsync(Guid categoryId, CreateProductDto productDto) {
        var validationResult = await _createValidator.ValidateAsync(productDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        var product = productDto.Adapt<Product>();

        product.Id = Guid.NewGuid();
        product.CategoryId = category.Id;

        await _productContext.AddAsync(product);
        await _productContext.SaveChangesAsync();

        var productToReturn = product.Adapt<ProductDto>();

        return productToReturn;
    }

    public async Task<ProductUpdateResponse> UpdateProductAsync(Guid categoryId, Guid productId, UpdateProductDto productDto) {
        var validationResult = await _updateValidator.ValidateAsync(productDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }


        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        var product = await _productContext.Product.SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var productEntity = productDto.Adapt(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<ProductDeleteResponse> DeleteProductAsync(Guid categoryId, Guid productId) {
        var category = await _productContext.Category.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) {
            return new NotFoundResponse(categoryId, nameof(Category));
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        _productContext.Product.Remove(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

}


[GenerateOneOf]
public partial class ProductsGetAllResponse : OneOfBase<(ProductLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}


[GenerateOneOf]
public partial class ProductUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ProductCreateResponse : OneOfBase<ProductDto, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ProductDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductGetResponse : OneOfBase<ProductDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductsGetResponse : OneOfBase<(IEnumerable<ProductDto> products, MetaData metaData)> {
}

