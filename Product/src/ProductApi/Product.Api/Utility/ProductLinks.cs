using Microsoft.Net.Http.Headers;
using ProductApi.Interfaces;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Utility;

public class ProductLinks : IProductLinks {
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<ProductDto> _dataShaper;

    public ProductLinks(LinkGenerator linkGenerator, IDataShaper<ProductDto> dataShaper) {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto, string fields, Guid categoryId,
        HttpContext httpContext) {
        var shapedProducts = ShapeData(productsDto, fields);

        if(ShouldGenerateLinks(httpContext))
            return ReturnLinkdedProducts(productsDto, fields, categoryId, httpContext, shapedProducts);

        return ReturnShapedProducts(shapedProducts);
    }

    private List<Entity> ShapeData(IEnumerable<ProductDto> productsDto, string fields) =>
        _dataShaper.ShapeData(productsDto, fields)
            .Select(e => e.Entity)
            .ToList();

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private LinkResponse ReturnShapedProducts(List<Entity> shapedProducts) =>
        new LinkResponse { ShapedEntities = shapedProducts };

    private LinkResponse ReturnLinkdedProducts(IEnumerable<ProductDto> productsDto,
        string fields, Guid categoryId, HttpContext httpContext, List<Entity> shapedProducts) {
        var productDtoList = productsDto.ToList();

        for(var index = 0; index < productDtoList.Count(); index++) {
            var productLinks = CreateLinksForProduct(httpContext, categoryId, productDtoList[index].Id, fields);
            shapedProducts[index].Add("Links", productLinks);
        }

        var productCollection = new LinkCollectionWrapper<Entity>(shapedProducts);
        var linkedProducts = CreateLinksForProducts(httpContext, productCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedProducts };
    }

    private List<Link> CreateLinksForProduct(HttpContext httpContext, Guid categoryId, Guid productid, string fields = "") {
        var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetProductForCategory", values: new { categoryId, productid, fields }),
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

    private LinkCollectionWrapper<Entity> CreateLinksForProducts(HttpContext httpContext,
        LinkCollectionWrapper<Entity> productsWrapper) {
        productsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetProductsForCategory", values: new { }),
                "self",
                "GET"));

        return productsWrapper;
    }
}

