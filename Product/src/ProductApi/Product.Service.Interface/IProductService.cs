using OneOf;
using OneOf.Types;
using ProductApi.Model.LinkModels;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Interfaces;

public interface IProductService {
    Task<ProductGetAllResponse> GetProductsAsync(Guid categoryId, LinkProductParameters linkParameters);
    Task<ProductGetResponse> GetProductByIdAsync(Guid categoryId, Guid productId);
    Task<ProductCreateResponse> CreateProductAsync(Guid categoryId, CreateProductDto productDto);
    Task<ProductUpdateResponse> UpdateProductAsync(Guid categoryId, Guid productId, UpdateProductDto productDto);
    Task<ProductDeleteResponse> DeleteProductAsync(Guid categoryId, Guid productId);
}



[GenerateOneOf]
public partial class ProductUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class ProductCreateResponse : OneOfBase<ProductDto, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class ProductDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductGetResponse : OneOfBase<ProductDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductGetAllResponse : OneOfBase<(LinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationFailed> {
}
