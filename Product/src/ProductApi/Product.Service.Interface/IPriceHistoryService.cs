using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceDtos;

namespace ProductApi.Interfaces;
public interface IPriceHistoryService {
    Task<(IEnumerable<PriceHistoryDto> pricesDto, MetaData metaData)> GetPricesAsync(Guid productId,
     PriceHistoryParameters priceParameters);
    Task<PriceHistoryDto> GetPriceByIdAsync(Guid priceId);
    Task<PriceHistoryDto> CreatePriceAsync(CreatePriceHistoryDto priceDto);
    Task UpdatePriceAsync(Guid priceId, UpdatePriceHistoryDto priceDto);
    Task DeletePriceAsync(Guid priceId);
}
