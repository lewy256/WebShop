using Microsoft.Net.Http.Headers;
using ProductApi.Shared.LinkModels;
using ProductApi.Shared.LinkModels.Products;
using ProductApi.Shared.ProductDtos;

namespace ProductApi.Infrastructure.Utility;

public interface IProductLinks {
    ProductLinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto, Guid cateogryId, HttpContext httpContext);
}

public class ProductLinks : IProductLinks {
    private readonly LinkGenerator _linkGenerator;

    public ProductLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public ProductLinkResponse TryGenerateLinks(IEnumerable<ProductDto> products, Guid categoryId,
        HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedProducts(products, categoryId, httpContext);
        }

        return new ProductLinkResponse { HasLinks = false, Products = products };
    }


    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private ProductLinkResponse ReturnLinkdedProducts(IEnumerable<ProductDto> products, Guid categoryId, HttpContext httpContext) {
        var linkedProducts = new List<LinkedProducts>();

        foreach(var product in products) {
            var links = CreateLinksForProduct(httpContext, categoryId, product.Id);

            linkedProducts.Add(new LinkedProducts(product, links));
        }

        var entity = new LinkedProductEntity() {
            Value = linkedProducts,
            Links = CreateLinksForProducts(httpContext)
        };

        return new ProductLinkResponse { HasLinks = true, LinkedEntities = entity };
    }
    private List<Link> CreateLinksForProduct(HttpContext httpContext, Guid categoryId, Guid productid) {
        var links = new List<Link>{
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetProductForCategory", values: new { categoryId, productid}),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteProductForCategory", values: new { categoryId, productid }),
                "delete_product",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateProductForCategory", values: new { categoryId, productid }),
                "update_product",
                "PUT")
            };
        return links;
    }

    private List<Link> CreateLinksForProducts(HttpContext httpContext) {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetProductsForCategory", values: new { }),
                "self",
                "GET")
            };
        return links;
    }
}

