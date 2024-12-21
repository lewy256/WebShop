using ProductApi.Shared.ProductDtos;

namespace ProductApi.Shared.LinkModels.Products;

public record LinkProductParameters(ProductParameters ProductParameters, HttpContext Context);
