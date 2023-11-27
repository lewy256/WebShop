﻿using ProductApi.Shared.Model;

namespace ProductApi.Interfaces;

public interface IProductService {
    Task<(IEnumerable<ProductDto> productsDto, MetaData metaData)> GetProductsAsync(int categoryId,
        ProductParameters productParameters);

    Task<(IEnumerable<ProductDto> productsDto, MetaData metaData)> GetProductsAsync(
        ProductParameters productParameters);

    Task<ProductDto> GetProductByIdAsync(Guid productId);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task UpdateProductAsync(Guid productId, UpdateProductDto productDto);
    Task DeleteProductAsync(Guid productId);
}