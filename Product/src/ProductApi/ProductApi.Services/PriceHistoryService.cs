using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Model.Responses;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Service;

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


    public async Task<PriceHistoryGetAllResponse> GetPricesHistoryAsync(Guid productId, LinkPriceHistoryParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.PriceHistoryParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }


        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var pricesHistory = await _productContext.PriceHistory
            .AsNoTracking()
            .Where(p => p.ProductId.Equals(productId))
            .SortPricesHistory(linkParameters.PriceHistoryParameters.OrderBy)
            .Skip((linkParameters.PriceHistoryParameters.PageNumber - 1) * linkParameters.PriceHistoryParameters.PageSize)
            .Take(linkParameters.PriceHistoryParameters.PageSize)
            .FilterPricesHistory(linkParameters.PriceHistoryParameters)
            .ToListAsync();

        var pricesHistoryDto = pricesHistory.Adapt<IEnumerable<PriceHistoryDto>>();


        var count = await _productContext.PriceHistory
            .Where(p => p.ProductId.Equals(productId))
            .CountAsync();

        var links = _priceHistoryLinks.TryGenerateLinks(pricesHistoryDto, productId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.PriceHistoryParameters.PageNumber,
            PageSize = linkParameters.PriceHistoryParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<PriceHistoryGetResponse> GetPriceHistoryByIdAsync(Guid productId, Guid priceHistoryId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var priceHistory = await _productContext.PriceHistory.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistory is null) {
            return new NotFoundResponse(priceHistoryId, nameof(priceHistory));
        }

        var priceHistoryDto = priceHistory.Adapt<PriceHistoryDto>();

        return priceHistoryDto;
    }

    public async Task<PriceHistoryCreateResponse> CreatePriceHistoryAsync(Guid productId, CreatePriceHistoryDto priceHistoryDto) {
        var validationResult = await _createValidator.ValidateAsync(priceHistoryDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
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
            return new ValidationFailed(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var priceHistory = await _productContext.PriceHistory.SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistory is null) {
            return new NotFoundResponse(priceHistoryId, nameof(priceHistory));
        }

        priceHistoryDto.Adapt(product);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<PriceHistoryDeleteResponse> DeletePriceHistoryAsync(Guid productId, Guid priceHistoryId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var priceHistory = await _productContext.PriceHistory.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(priceHistoryId));

        if(priceHistory is null) {
            return new NotFoundResponse(priceHistoryId, nameof(priceHistory));
        }

        _productContext.PriceHistory.Remove(priceHistory);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
}