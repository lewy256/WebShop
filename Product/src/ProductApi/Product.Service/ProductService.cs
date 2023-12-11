﻿using Mapster;
using Microsoft.EntityFrameworkCore;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.Exceptions;
using ProductApi.Shared.Model;

namespace ProductApi.Service;

public class ProductService : IProductService {
    private readonly ProductContext _productContext;


    public ProductService(ProductContext productContext) {
        _productContext = productContext;
    }


    /*   [HttpGet]
       [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
       public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
       [FromQuery] EmployeeParameters employeeParameters) {
           var linkParams = new LinkParameters(employeeParameters, HttpContext);
           var pagedResult = await _service.EmployeeService.GetEmployeesAsync(companyId,
           linkParams, trackChanges: false);
           Response.Headers.Add("X-Pagination",
           JsonSerializer.Serialize(pagedResult.metaData));
           return Ok(pagedResult.employees);
       }*/


    public async Task<(IEnumerable<ProductDto> productsDto, MetaData metaData)> GetProductsAsync(Guid categoryId,
        ProductParameters productParameters) {
        if(!productParameters.ValidPriceRange) throw new MaxPriceRangeBadRequestException();


        await CheckIfCategoryExists(categoryId);

        var products = await _productContext.Product
            .Where(p => p.CategoryId.Equals(categoryId) && p.Price >= productParameters.MinPrice &&
                        p.Price <= productParameters.MaxPrice)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .ToListAsync();

        var productsDto = products.Adapt<IEnumerable<ProductDto>>();

        var count = await _productContext.Product
            .Where(p => p.CategoryId.Equals(categoryId))
            .CountAsync();


        return (productsDto: productsDto, metaData: new MetaData() {
            CurrentPage = productParameters.PageNumber,
            PageSize = productParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductDto> GetProductByIdAsync(Guid productId) {
        var product = await GetProductAndCheckIfItExists(productId, false);

        var productDto = product.Adapt<ProductDto>();

        return productDto;
    }

    public async Task<(IEnumerable<ProductDto> productsDto, MetaData metaData)> GetProductsAsync(
        ProductParameters productParameters) {
        if(!productParameters.ValidPriceRange) throw new MaxPriceRangeBadRequestException();


        var products = await _productContext.Product
            .Where(p => p.Price >= productParameters.MinPrice && p.Price <= productParameters.MaxPrice)
            .Skip((productParameters.PageNumber - 1) * productParameters.PageSize)
            .Take(productParameters.PageSize)
            .ToListAsync();

        var productsDto = products.Adapt<IEnumerable<ProductDto>>();

        var count = await _productContext.Product
            .CountAsync();


        return (productsDto: productsDto, metaData: new MetaData() {
            CurrentPage = productParameters.PageNumber,
            PageSize = productParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto) {
        var product = productDto.Adapt<Product>();

        product.Id = Guid.NewGuid();

        await _productContext.AddAsync(product);
        await _productContext.SaveChangesAsync();

        var productToReturn = product.Adapt<ProductDto>();

        return productToReturn;
    }

    public async Task UpdateProductAsync(Guid productId, UpdateProductDto productDto) {
        var product = await GetProductAndCheckIfItExists(productId, true);

        productDto.Adapt(product);

        await _productContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid productId) {
        var product = await GetProductAndCheckIfItExists(productId, false);

        _productContext.Product.Remove(product);

        await _productContext.SaveChangesAsync();
    }


    private async Task CheckIfCategoryExists(Guid categoryId) {
        var category = await _productContext.Category.SingleOrDefaultAsync(c => c.Id.Equals(categoryId));

        if(category is null) throw new CategoryNotFoundException(categoryId);
    }

    private async Task<Product> GetProductAndCheckIfItExists(Guid productId, bool trackChanges) {
        var product = trackChanges
            ? await _productContext.Product
                .SingleOrDefaultAsync(p => p.Id.Equals(productId))
            : await _productContext.Product
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) throw new ProductNotFoundException(productId);

        return product;
    }

    /*  public Task<(LinkResponse linkResponse, MetaData metaData)> GetProductsAsync(Guid cateogryId, ProductParameters productParameters) {
          throw new NotImplementedException();
      }*/
}