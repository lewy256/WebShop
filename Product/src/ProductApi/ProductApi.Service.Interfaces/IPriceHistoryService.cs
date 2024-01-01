using OneOf;
using OneOf.Types;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Service.Interfaces;
public interface IPriceHistoryService {
    Task<PriceHistoryGetAllResponse> GetPricesHistoryAsync(Guid productId, LinkPriceHistoryParameters priceParameters);
    Task<PriceHistoryGetResponse> GetPriceHistoryByIdAsync(Guid productId, Guid priceHistoryId);
    Task<PriceHistoryCreateResponse> CreatePriceHistoryAsync(Guid productId, CreatePriceHistoryDto priceDto);
    Task<PriceHistoryUpdateResponse> UpdatePriceHistoryAsync(Guid productId, Guid priceHistoryId, UpdatePriceHistoryDto priceDto);
    Task<PriceHistoryDeleteResponse> DeletePriceHistoryAsync(Guid productId, Guid priceHistoryId);
}

[GenerateOneOf]
public partial class PriceHistoryUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class PriceHistoryCreateResponse : OneOfBase<PriceHistoryDto, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class PriceHistoryDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class PriceHistoryGetResponse : OneOfBase<PriceHistoryDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class PriceHistoryGetAllResponse : OneOfBase<(PriceHistoryLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationFailed> {
}
