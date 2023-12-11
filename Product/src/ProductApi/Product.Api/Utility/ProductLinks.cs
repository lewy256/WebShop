using ProductApi.Interfaces;
using ProductApi.Model.LinkModels;
using ProductApi.Shared.Model;

namespace ProductApi.Utility;

public class ProductLinks : IProductLinks {
    private readonly LinkGenerator _linkGenerator;

    public ProductLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto, string fields, Guid cateogryId, HttpContext httpContext) {
        throw new NotImplementedException();
    }
}

