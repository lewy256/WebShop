using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Model.LinkModels.PriceHistory;
public class PriceHistoryLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<PriceHistoryDto> PricesHistoryDto { get; set; }
    public LinkedPriceHistoryEntity LinkedEntity { get; set; }
}
