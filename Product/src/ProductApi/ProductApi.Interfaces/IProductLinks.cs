using Microsoft.AspNetCore.Http;
using ProductApi.Model.LinkModels.Products;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Interfaces;
public interface IProductLinks {
    ProductLinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto,
        string fields, Guid cateogryId, HttpContext httpContext);

}
