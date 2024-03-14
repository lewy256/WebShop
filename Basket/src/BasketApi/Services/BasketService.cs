using BasketApi.Consumers.Messages;
using BasketApi.Models;
using BasketApi.Responses;
using BasketApi.Shared;
using FluentValidation;
using Mapster;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using OneOf;
using OneOf.Types;
using System.Security.Claims;
using System.Text.Json;

namespace BasketApi.Services;

public class BasketService {
    private readonly IDistributedCache _redisCache;
    private readonly IValidator<UpdateBasketDto> _validator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasketService(IDistributedCache cache, IValidator<UpdateBasketDto> validator,
        IPublishEndpoint publishEndpoint,
        IHttpContextAccessor httpContextAccessor) {
        _redisCache = cache;
        _validator = validator;
        _publishEndpoint = publishEndpoint;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BasketGetResponse> GetBasketAsync(Guid basketId) {
        var value = await _redisCache.GetStringAsync(basketId.ToString());

        if(String.IsNullOrEmpty(value)) {
            var basketDto = await UpdateBasketAsync(new UpdateBasketDto() {
                Id = basketId,
                Items = []
            });
            return basketDto.AsT0;

        }

        var basket = JsonSerializer.Deserialize<Basket>(value);

        var basketToReturn = basket.Adapt<BasketDto>();

        return basketToReturn;
    }

    public async Task<BasketUpdateResponse> UpdateBasketAsync(UpdateBasketDto basket) {
        var validationResult = await _validator.ValidateAsync(basket);

        if(!validationResult.IsValid) {

            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            return new ValidationResponse(vaildationFailed);
        }

        var basketToSave = basket.Adapt<Basket>();

        decimal totalprice = 0;

        foreach(var item in basketToSave.Items) {
            totalprice += item.Price * item.Quantity;
        }

        basketToSave.TotalPrice = totalprice;

        await _redisCache.SetStringAsync(basketToSave.Id.ToString(), JsonSerializer.Serialize(basketToSave));

        var basketToReturn = basketToSave.Adapt<BasketDto>();

        return basketToReturn;
    }

    public async Task<BasketCheckoutResponse> CheckoutBasketAsync(Guid basketId) {
        var basketEntity = await GetBasketAsync(basketId);

        if(basketEntity.AsT0.Items.Count == 0) {
            return new BadRequestResponse($"The basket with id: {basketId} is empty.");
        }

        var basket = basketEntity.Adapt<Basket>();

        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        basket.UserId = new Guid(userId);

        await _publishEndpoint.Publish(new BasketCreated {
            Basket = basket
        });

        return new Success();
    }

    public async Task<BasketDeleteResponse> DeleteBasketAsync(Guid basketId) {
        var basket = await _redisCache.GetStringAsync(basketId.ToString());

        if(String.IsNullOrEmpty(basket)) {
            return new NotFoundResponse(basketId, nameof(Basket));
        }

        await _redisCache.RemoveAsync(basketId.ToString());

        return new Success();
    }
}

[GenerateOneOf]
public partial class BasketCheckoutResponse : OneOfBase<Success, BadRequestResponse> {
}
[GenerateOneOf]
public partial class BasketGetResponse : OneOfBase<BasketDto> {
}
[GenerateOneOf]
public partial class BasketDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketUpdateResponse : OneOfBase<BasketDto, ValidationResponse> {
}
