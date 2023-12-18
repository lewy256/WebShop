using ProductApi.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceDtos;

namespace ProductApi.Service;

public class PriceHistoryService : IPriceHistoryService {
    public Task<PriceHistoryDto> CreatePriceAsync(CreatePriceHistoryDto priceDto) {
        throw new NotImplementedException();
    }

    public Task DeletePriceAsync(Guid priceId) {
        throw new NotImplementedException();
    }

    public Task<PriceHistoryDto> GetPriceByIdAsync(Guid priceId) {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<PriceHistoryDto> pricesDto, MetaData metaData)> GetPricesAsync(Guid productId, PriceHistoryParameters priceParameters) {
        throw new NotImplementedException();
    }

    public Task UpdatePriceAsync(Guid priceId, UpdatePriceHistoryDto priceDto) {
        throw new NotImplementedException();
    }
}