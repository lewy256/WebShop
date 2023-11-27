using BasketApi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BasketApi.Services;

public class BasketService {
    private readonly IDistributedCache _redisCache;

    public BasketService(IDistributedCache cache) {
        _redisCache = cache;
    }

    public async Task<Basket> GetBasket(Guid basketId) {
        var basket = await _redisCache.GetStringAsync(basketId.ToString());

        if(String.IsNullOrEmpty(basket)) {
            return null;
        }

        return JsonConvert.DeserializeObject<Basket>(basket);
    }

    public async Task<Basket> CreateBasket(Basket basket) {
        await _redisCache.SetStringAsync(basket.Id.ToString(), JsonConvert.SerializeObject(basket));

        return await GetBasket(basket.Id);
    }

    public async Task DeleteBasket(Guid basketId) {
        await _redisCache.RemoveAsync(basketId.ToString());
    }
}