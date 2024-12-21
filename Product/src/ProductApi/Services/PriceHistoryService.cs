using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Extensions;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Utility;
using ProductApi.Shared;
using ProductApi.Shared.LinkModels.PriceHistory;
using ProductApi.Shared.PriceHistoryDtos;
using ProductApi.Shared.Responses;

namespace ProductApi.Services;

public interface IPriceHistoryService {
    Task<PricesHistoryGetAllResponse> GetPriceHistoriesAsync(Guid productId, LinkPriceHistoryParameters priceHistoryParameters);
    Task<PriceHistoryGetResponse> GetPriceHistoryByIdAsync(Guid productId, Guid priceHistoryId);
    Task<PriceHistoryCreateResponse> CreatePriceHistoryAsync(Guid productId, CreatePriceHistoryDto priceDto);
    Task<PriceHistoryUpdateResponse> UpdatePriceHistoryAsync(Guid productId, Guid priceHistoryId, UpdatePriceHistoryDto priceDto);
    Task<PriceHistoryDeleteResponse> DeletePriceHistoryAsync(Guid productId, Guid priceHistoryId);
}


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

    public async Task<PricesHistoryGetAllResponse> GetPriceHistoriesAsync(Guid productId, LinkPriceHistoryParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.PriceHistoryParameters);

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
            .FilterPriceHistories(linkParameters.PriceHistoryParameters)
            .SortPricesHistory(linkParameters.PriceHistoryParameters.OrderBy)
            .Skip((linkParameters.PriceHistoryParameters.PageNumber - 1) * linkParameters.PriceHistoryParameters.PageSize)
            .Take(linkParameters.PriceHistoryParameters.PageSize)
            .ProjectToType<PriceHistoryDto>()
            .ToListAsync();


        var count = await query.CountAsync();

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

[GenerateOneOf]
public partial class PricesHistoryGetAllResponse : OneOfBase<(PriceHistoryLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}


[GenerateOneOf]
public partial class PriceHistoryUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class PriceHistoryCreateResponse : OneOfBase<PriceHistoryDto, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class PriceHistoryDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class PriceHistoryGetResponse : OneOfBase<PriceHistoryDto, NotFoundResponse> {
}
