using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Products;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.Responses;
using ProductParameters = ProductApi.Shared.Model.ProductDtos.V2.ProductParameters;

namespace ProductApi.Service.V2;

public class ProductService {
    private readonly ProductContext _productContext;
    private readonly IProductLinks _productLinks;
    private readonly IValidator<ProductParameters> _parametersValidator;
    private readonly IFileService _fileService;

    public ProductService(ProductContext productContext,
        IProductLinks productLinks,
        IValidator<ProductParameters> parametersValidator,
        IFileService fileService
        ) {
        _productContext = productContext;
        _productLinks = productLinks;
        _parametersValidator = parametersValidator;
        _fileService = fileService;
    }


    public async Task<ProductGetAllResponse> GetProductsAsync(Guid categoryId, LinkProductParameters linkParameters) {
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
            .SortProducts(linkParameters.ProductParameters.OrderBy)
            .Skip((linkParameters.ProductParameters.PageNumber - 1) * linkParameters.ProductParameters.PageSize)
            .Take(linkParameters.ProductParameters.PageSize)
            .FilterProducts(linkParameters.ProductParameters)
            .SearchProducts(linkParameters.ProductParameters.SearchTerm)
            .ToListAsync();

        var count = await query.CountAsync();

        var productsDto = products.Adapt<List<ProductDto>>();

        for(int i = 0; i < products.Count; i++) {
            var images = products[i].Images;
            var uris = _fileService.GetUrisForImages(images);
            productsDto[i].ImageUris.AddRange(uris.AsT0);
        }

        var links = _productLinks.TryGenerateLinks(productsDto, linkParameters.ProductParameters.Fields, categoryId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ProductParameters.PageNumber,
            PageSize = linkParameters.ProductParameters.PageSize,
            TotalCount = count
        });
    }

}
[GenerateOneOf]
public partial class ProductGetAllResponse : OneOfBase<(ProductLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}