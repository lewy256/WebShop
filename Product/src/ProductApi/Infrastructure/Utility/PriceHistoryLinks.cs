using Microsoft.Net.Http.Headers;
using ProductApi.Shared.LinkModels;
using ProductApi.Shared.LinkModels.PriceHistory;
using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Infrastructure.Utility;
public interface IPriceHistoryLinks {
    PriceHistoryLinkResponse TryGenerateLinks(IEnumerable<PriceHistoryDto> priceHistoriesDto, Guid productId, HttpContext httpContext);
}

public class PriceHistoryLinks : IPriceHistoryLinks {
    private readonly LinkGenerator _linkGenerator;

    public PriceHistoryLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public PriceHistoryLinkResponse TryGenerateLinks(IEnumerable<PriceHistoryDto> priceHistories, Guid productId, HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedPricesHistory(priceHistories, productId, httpContext);
        }

        return new PriceHistoryLinkResponse() { HasLinks = false, PricesHistoryDto = priceHistories };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private PriceHistoryLinkResponse ReturnLinkdedPricesHistory(IEnumerable<PriceHistoryDto> priceHistories, Guid productId, HttpContext httpContext) {
        var linkedPricesHistory = new List<LinkedPriceHistories>();

        foreach(var price in priceHistories) {
            var links = CreateLinksForPriceHistory(httpContext, productId, price.Id);

            linkedPricesHistory.Add(new LinkedPriceHistories(price, links));
        }

        var entity = new LinkedPriceHistoryEntity() {
            Value = linkedPricesHistory,
            Links = CreateLinksPricesHsitory(httpContext)
        };

        return new PriceHistoryLinkResponse { HasLinks = true, LinkedEntity = entity };
    }

    private List<Link> CreateLinksForPriceHistory(HttpContext httpContext, Guid productId, Guid priceHistoryId) {
        var links = new List<Link>{
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetPriceHsitory", values: new { productId,priceHistoryId}),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeletePriceHsitory", values: new { productId,priceHistoryId }),
                "delete_priceHistory",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdatePriceHsitory", values: new { productId,priceHistoryId}),
                "update_priceHisotry",
                "PUT")
            };
        return links;
    }

    private List<Link> CreateLinksPricesHsitory(HttpContext httpContext) {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetPricesHistory", values: new { }),
                "self",
                "GET")
            };
        return links;
    }

}