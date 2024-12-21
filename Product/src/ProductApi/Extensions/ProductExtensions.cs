using ProductApi.Entities;
using ProductApi.Shared.ProductDtos;
using System.Linq.Expressions;

namespace ProductApi.Extensions;

public static class ProductExtensions {
    public static IQueryable<Product> SearchProducts(this IQueryable<Product> products, string? searchTerm) {
        if(string.IsNullOrWhiteSpace(searchTerm)) {
            return products;
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return products.Where(e => e.ProductName.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Product> FilterProducts(this IQueryable<Product> products, ProductParameters productParameters) {
        return products.Where(p =>
            (!productParameters.MinPrice.HasValue || p.Price >= productParameters.MinPrice) &&
            (!productParameters.MaxPrice.HasValue || p.Price <= productParameters.MaxPrice)
        );
    }

    public static IQueryable<Product> SortProducts(this IQueryable<Product> products, string? queryString) {
        if(string.IsNullOrWhiteSpace(queryString)) {
            return products.OrderBy(p => p.ProductName);
        }

        //To order by more than one property, it is necessary to create a composite index.
        var column = queryString.Trim().ToLower().Split(',')[0];
        var direction = column.EndsWith(" desc") ? " desc" : " asc";
        column = column.Replace(direction, "");

        Expression<Func<Product, object>> keySelector = column switch {
            "price" => product => product.Price,
            _ => product => product.ProductName
        };

        if(direction.Equals(" desc")) {
            return products.OrderByDescending(keySelector);
        }
        else {
            return products.OrderBy(keySelector);
        }
    }
}
