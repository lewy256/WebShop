using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Service.Extensions;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceHistoryDtos;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.V2;

public class PriceHistoryService {
    private readonly ProductContext _productContext;
    private readonly IPriceHistoryLinks _priceHistoryLinks;
    private readonly IValidator<PriceHistoryParameters> _parametersValidator;

    public PriceHistoryService(ProductContext productContext,
        IPriceHistoryLinks priceHistoryLinks,
        IValidator<PriceHistoryParameters> parametersValidator) {
        _productContext = productContext;
        _priceHistoryLinks = priceHistoryLinks;
        _parametersValidator = parametersValidator;
    }
    public async Task<PriceHistoryGetAllResponse> GetPricesHistoryAsync(Guid productId, LinkPriceHistoryParameters linkParameters) {
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
            .SortPricesHistory(linkParameters.PriceHistoryParameters.OrderBy)
            .Skip((linkParameters.PriceHistoryParameters.PageNumber - 1) * linkParameters.PriceHistoryParameters.PageSize)
            .Take(linkParameters.PriceHistoryParameters.PageSize)
            .FilterPricesHistory(linkParameters.PriceHistoryParameters)
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
}
[GenerateOneOf]
public partial class PriceHistoryGetAllResponse : OneOfBase<(PriceHistoryLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}