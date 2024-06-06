using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.Responses;
using ProductParameters = ProductApi.Shared.Model.ProductDtos.V1.ProductParameters;

namespace ProductApi.Service.V1;

public class ProductService : IProductService {
    private readonly ProductContext _productContext;
    private readonly IProductLinks _productLinks;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly IValidator<ProductParameters> _parametersValidator;
    private readonly IFileService _fileService;

    public ProductService(ProductContext productContext,
        IProductLinks productLinks,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        IValidator<ProductParameters> parametersValidator,
        IFileService fileService
        ) {
        _productContext = productContext;
        _productLinks = productLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
        _fileService = fileService;
    }

    public async Task<ProductsGetResponse> GetProductsAsync(Guid categoryId, ProductParameters productParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(productParameters);

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
            .SortProducts(productParameters.OrderBy)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .FilterProducts(productParameters)
            .SearchProducts(productParameters.SearchTerm)
            .ToListAsync();

        var count = await query.CountAsync();

        var productsDto = products.Adapt<List<ProductDto>>();

        for(int i = 0; i < products.Count; i++) {
            var images = products[i].Images;
            var uris = _fileService.GetUrisForImages(images);
            productsDto[i].ImageUris.AddRange(uris.AsT0);
        }

        return (products: productsDto, metaData: new MetaData() {
            CurrentPage = productParameters.PageNumber,
            PageSize = productParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductsGetAllResponse> GetProductsAsync(ProductParameters productParameters) {
        var query = _productContext.Product.AsNoTracking();

        var products = await query
            .SortProducts(productParameters.OrderBy)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .FilterProducts(productParameters)
            .SearchProducts(productParameters.SearchTerm)
            .ToListAsync();

        var count = await query.CountAsync();

        var productsDto = products.Adapt<List<ProductDto>>();

        for(int i = 0; i < products.Count; i++) {
            var images = products[i].Images;
            var uris = _fileService.GetUrisForImages(images);
            productsDto[i].ImageUris.AddRange(uris.AsT0);
        }

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

        var product = await _productContext.Product.AsNoTracking().Include(i => i.Images).SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var productDto = product.Adapt<ProductDto>();

        var uris = _fileService.GetUrisForImages(product.Images);

        productDto.ImageUris.AddRange(uris.AsT0);

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