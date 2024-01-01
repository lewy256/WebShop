using Microsoft.Net.Http.Headers;
using ProductApi.Interfaces;
using ProductApi.Model.LinkModels;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Utility;

public class PriceHistoryLinks : IPriceHistoryLinks {
    private readonly LinkGenerator _linkGenerator;

    public PriceHistoryLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public PriceHistoryLinkResponse TryGenerateLinks(IEnumerable<PriceHistoryDto> pricesHistory, Guid productId, HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedPricesHistory(pricesHistory, productId, httpContext);
        }

        return new PriceHistoryLinkResponse() { HasLinks = false, PricesHistoryDto = pricesHistory };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private PriceHistoryLinkResponse ReturnLinkdedPricesHistory(IEnumerable<PriceHistoryDto> pricesHistory, Guid productId, HttpContext httpContext) {
        var pricesHistoryList = pricesHistory.ToList();
        var linkedPricesHistory = new List<LinkedPricesHistory>();

        foreach(var item in pricesHistoryList) {
            var pricesHistoryLinks = CreateLinksForPriceHistory(httpContext, productId, item.Id);

            linkedPricesHistory.Add(new LinkedPricesHistory() {
                Id = item.Id,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                PriceValue = item.PriceValue,
                Links = pricesHistoryLinks
            });
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