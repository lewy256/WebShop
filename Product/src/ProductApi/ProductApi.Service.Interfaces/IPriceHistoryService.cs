using OneOf;
using OneOf.Types;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.PriceHistoryDtos;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.Interfaces;
public interface IPriceHistoryService {
    Task<PriceHistoryGetAllResponse> GetPricesHistoryAsync(Guid productId, PriceHistoryParameters priceHistoryParameters);
    Task<PriceHistoryGetResponse> GetPriceHistoryByIdAsync(Guid productId, Guid priceHistoryId);
    Task<PriceHistoryCreateResponse> CreatePriceHistoryAsync(Guid productId, CreatePriceHistoryDto priceDto);
    Task<PriceHistoryUpdateResponse> UpdatePriceHistoryAsync(Guid productId, Guid priceHistoryId, UpdatePriceHistoryDto priceDto);
    Task<PriceHistoryDeleteResponse> DeletePriceHistoryAsync(Guid productId, Guid priceHistoryId);
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


[GenerateOneOf]
public partial class PriceHistoryGetAllResponse : OneOfBase<(IEnumerable<PriceHistoryDto> pricesHistory, MetaData metaData), NotFoundResponse, ValidationResponse> {
}
