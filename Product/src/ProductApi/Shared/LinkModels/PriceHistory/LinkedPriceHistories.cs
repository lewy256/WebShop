using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Shared.LinkModels.PriceHistory;
public record LinkedPriceHistories {
    public LinkedPriceHistories(PriceHistoryDto priceHistory, List<Link> links) {
        Id = priceHistory.Id;
        StartDate = priceHistory.StartDate;
        EndDate = priceHistory.EndDate;
        PriceValue = priceHistory.PriceValue;
        Links = links;
    }

    public LinkedPriceHistories() { }

    public Guid Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal PriceValue { get; init; }
    public List<Link> Links { get; init; }
}
