using Microsoft.AspNetCore.Http;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Interfaces;

public interface IPriceHistoryLinks {
    PriceHistoryLinkResponse TryGenerateLinks(IEnumerable<PriceHistoryDto> priceHistoryDto, Guid productId, HttpContext httpContext);
}