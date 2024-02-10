using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceHistoryDtos;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.V1;

public class PriceHistoryService : IPriceHistoryService {
    private readonly ProductContext _productContext;
    private readonly IPriceHistoryLinks _priceHistoryLinks;
    private readonly IValidator<CreatePriceHistoryDto> _createValidator;
    private readonly IValidator<UpdatePriceHistoryDto> _updateValidator;
    private readonly IValidator<PriceHistoryParameters> _parametersValidator;

    public PriceHistoryService(ProductContext productContext, IPriceHistoryLinks priceHistoryLinks,
        IValidator<CreatePriceHistoryDto> createValidator,
        IValidator<UpdatePriceHistoryDto> updateValidator,
        IValidator<PriceHistoryParameters> parametersValidator) {
        _productContext = productContext;
        _priceHistoryLinks = priceHistoryLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
    }

    public async Task<PriceHistoryGetAllResponse> GetPricesHistoryAsync(Guid productId, PriceHistoryParameters priceHistoryParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(priceHistoryParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var query = _productContext.PriceHistory
            .AsNoTracking()
            .Where(p => p.ProductId.Equals(productId));

        var pricesHistoryDto = await query
            .SortPricesHistory(priceHistoryParameters.OrderBy)
            .Skip((priceHistoryParameters.PageNumber - 1) * priceHistoryParameters.PageSize)
            .Take(priceHistoryParameters.PageSize)
            .FilterPricesHistory(priceHistoryParameters)
            .ProjectToType<PriceHistoryDto>()
            .ToListAsync();

        var count = await query.CountAsync();

        return (pricesHistory: pricesHistoryDto, metaData: new MetaData() {
            CurrentPage = priceHistoryParameters.PageNumber,
            PageSize = priceHistoryParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<PriceHistoryGetResponse> GetPriceHistoryByIdAsync(Guid productId, Guid priceHistoryId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var priceHistoryDto = await _productContext.PriceHistory.AsNoTracking().ProjectToType<PriceHistoryDto>().SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistoryDto is null) {
            return new NotFoundResponse(priceHistoryId, nameof(PriceHistory));
        }

        return priceHistoryDto;
    }

    public async Task<PriceHistoryCreateResponse> CreatePriceHistoryAsync(Guid productId, CreatePriceHistoryDto priceHistoryDto) {
        var validationResult = await _createValidator.ValidateAsync(priceHistoryDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var priceHistory = priceHistoryDto.Adapt<PriceHistory>();

        priceHistory.Id = Guid.NewGuid();
        priceHistory.ProductId = product.Id;
        priceHistory.Discriminator = nameof(PriceHistory);

        await _productContext.AddAsync(priceHistory);
        await _productContext.SaveChangesAsync();

        var priceHistoryToReturn = priceHistory.Adapt<PriceHistoryDto>();

        return priceHistoryToReturn;
    }

    public async Task<PriceHistoryUpdateResponse> UpdatePriceHistoryAsync(Guid productId, Guid priceHistoryId, UpdatePriceHistoryDto priceHistoryDto) {
        var validationResult = await _updateValidator.ValidateAsync(priceHistoryDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var priceHistory = await _productContext.PriceHistory.SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistory is null) {
            return new NotFoundResponse(priceHistoryId, nameof(PriceHistory));
        }

        priceHistoryDto.Adapt(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<PriceHistoryDeleteResponse> DeletePriceHistoryAsync(Guid productId, Guid priceHistoryId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var priceHistory = await _productContext.PriceHistory.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistory is null) {
            return new NotFoundResponse(priceHistoryId, nameof(PriceHistory));
        }

        _productContext.PriceHistory.Remove(priceHistory);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
}