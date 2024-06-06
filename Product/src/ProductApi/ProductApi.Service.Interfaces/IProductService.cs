using OneOf;
using OneOf.Types;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.ProductDtos.V1;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.Interfaces;

public interface IProductService {
    Task<ProductsGetResponse> GetProductsAsync(Guid categoryId, ProductParameters productParameters);
    Task<ProductsGetAllResponse> GetProductsAsync(ProductParameters productParameters);
    Task<ProductGetResponse> GetProductByIdAsync(Guid categoryId, Guid productId);
    Task<ProductCreateResponse> CreateProductAsync(Guid categoryId, CreateProductDto productDto);
    Task<ProductUpdateResponse> UpdateProductAsync(Guid categoryId, Guid productId, UpdateProductDto productDto);
    Task<ProductDeleteResponse> DeleteProductAsync(Guid categoryId, Guid productId);
}



[GenerateOneOf]
public partial class ProductUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ProductCreateResponse : OneOfBase<ProductDto, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ProductDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductGetResponse : OneOfBase<ProductDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ProductsGetResponse : OneOfBase<(IEnumerable<ProductDto> products, MetaData metaData), NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ProductsGetAllResponse : OneOfBase<(IEnumerable<ProductDto> products, MetaData metaData)> {
}