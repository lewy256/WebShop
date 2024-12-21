using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Shared.LinkModels.PriceHistory;
public class PriceHistoryLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<PriceHistoryDto> PricesHistoryDto { get; set; }
    public LinkedPriceHistoryEntity LinkedEntity { get; set; }
}
