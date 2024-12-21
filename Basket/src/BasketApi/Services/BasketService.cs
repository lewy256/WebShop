using BasketApi.Entities;
using BasketApi.Responses;
using BasketApi.Shared;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using OneOf;
using OneOf.Types;
using System.Security.Claims;
using System.Text.Json;

namespace BasketApi.Services;

public class BasketService {
    private readonly IDistributedCache _redisCache;
    private readonly IValidator<UpsertBasketDto> _updateValidator;
    private IHttpContextAccessor _httpContextAccessor;

    public BasketService(
        IDistributedCache cache,
        IValidator<UpsertBasketDto> updateValidator,
        IHttpContextAccessor httpContextAccessor) {
        _redisCache = cache;
        _updateValidator = updateValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BasketGetResponse> GetBasketAsync() {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        string? document = await _redisCache.GetStringAsync(userId);

        if(String.IsNullOrEmpty(document)) {
            return new NotFoundResponse(nameof(Basket));
        }

        var basket = JsonSerializer.Deserialize<Basket>(document);

        var basketToReturn = basket.Adapt<BasketDto>();

        return basketToReturn;
    }

    private decimal CountTotalPrice(List<BasketItem> items) {
        decimal totalprice = 0;

        foreach(var item in items) {
            totalprice += item.Price * item.Quantity;
        }

        return totalprice;
    }

    public async Task<BasketUpsertResponse> UpsertBasketAsync(UpsertBasketDto basketDto) {
        var validationResult = await _updateValidator.ValidateAsync(basketDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Select(x => new ValidationError() {
                PropertyName = x.PropertyName.Split('.')[1],
                ErrorMessage = x.ErrorMessage
            });

            return new ValidationResponse(vaildationFailed);
        }

        string userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        string? document = await _redisCache.GetStringAsync(userId);

        Basket basket;

        if(String.IsNullOrEmpty(document)) {
            basket = new Basket() {
                Id = userId,
                Items = basketDto.Items,
                TotalPrice = CountTotalPrice(basketDto.Items)
            };
        }
        else {
            basket = JsonSerializer.Deserialize<Basket>(document);
            basket.Items = basketDto.Items;
            basket.TotalPrice = CountTotalPrice(basketDto.Items);
        }

        await _redisCache.SetStringAsync(basket.Id.ToString(), JsonSerializer.Serialize(basket));

        var basketToReturn = basket.Adapt<BasketDto>();

        return basketToReturn;
    }

    public async Task<BasketDeleteResponse> DeleteBasketAsync() {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var document = await _redisCache.GetStringAsync(userId);

        if(String.IsNullOrEmpty(document)) {
            return new NotFoundResponse(nameof(Basket));
        }

        await _redisCache.RemoveAsync(userId);

        return new Success();
    }
}


[GenerateOneOf]
public partial class BasketGetResponse : OneOfBase<BasketDto, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketUpsertResponse : OneOfBase<BasketDto, ValidationResponse, NotFoundResponse> {
}