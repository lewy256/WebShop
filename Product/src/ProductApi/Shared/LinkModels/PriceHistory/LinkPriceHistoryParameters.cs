using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Shared.LinkModels.PriceHistory;

public record LinkPriceHistoryParameters(PriceHistoryParameters PriceHistoryParameters, HttpContext Context);
