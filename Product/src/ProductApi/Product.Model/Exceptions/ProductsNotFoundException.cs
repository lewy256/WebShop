namespace ProductApi.Model.Exceptions;

public sealed class ProductsNotFoundException : NotFoundException {
    public ProductsNotFoundException(int categoryId)
        : base($"Products with category id: {categoryId} don't exist in the database.") {
    }
}