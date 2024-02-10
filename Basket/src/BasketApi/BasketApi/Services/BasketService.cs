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
    private readonly IValidator<CreateBasketDto> _validator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasketService(IDistributedCache cache, IValidator<CreateBasketDto> validator,
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
            return new NotFoundResponse(basketId, nameof(Basket));
        }

        var basket = JsonSerializer.Deserialize<Basket>(value);

        var basketToReturn = basket.Adapt<BasketDto>();

        return basketToReturn;
    }

    public async Task<BasketUpdateResponse> CreateBasketAsync(CreateBasketDto basket) {
        if(basket is null) {
            return new BadRequestResponse();
        }

        var validationResult = await _validator.ValidateAsync(basket);

        if(!validationResult.IsValid) {

            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

            return new ValidationResponse(vaildationFailed);
        }

        var basketToSave = basket.Adapt<Basket>();

        var basketEntity = await GetBasketAsync(basket.Id);

        if(basketEntity.IsT0) {
            basketToSave.Id = Guid.NewGuid();
        }

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

        if(basketEntity.IsT1) {
            return basketEntity.AsT1;
        }

        var basket = basketEntity.Adapt<Basket>();

        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        basket.UserId = new Guid(userId);

        //basket.UserId = userId is null ? Guid.NewGuid() : new Guid(userId);

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
public partial class BasketCheckoutResponse : OneOfBase<Success, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketGetResponse : OneOfBase<BasketDto, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}
[GenerateOneOf]
public partial class BasketUpdateResponse : OneOfBase<BasketDto, ValidationResponse, BadRequestResponse> {
}
