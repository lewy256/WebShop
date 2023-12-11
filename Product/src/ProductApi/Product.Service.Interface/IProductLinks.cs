using Microsoft.AspNetCore.Http;
using ProductApi.Model.LinkModels;
using ProductApi.Shared.Model;

namespace ProductApi.Interfaces;
public interface IProductLinks {
    LinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto,
        string fields, Guid cateogryId, HttpContext httpContext);

}
