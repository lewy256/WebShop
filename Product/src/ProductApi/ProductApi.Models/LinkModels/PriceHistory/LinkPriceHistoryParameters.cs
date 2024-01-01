using Microsoft.AspNetCore.Http;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Model.LinkModels.PriceHistory;

public record LinkPriceHistoryParameters(PriceHistoryParameters PriceHistoryParameters, HttpContext Context);
