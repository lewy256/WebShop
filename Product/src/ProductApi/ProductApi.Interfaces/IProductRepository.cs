using ProductApi.Model.Entities;
using System.Linq.Expressions;

namespace ProductApi.Interfaces;

public interface IProductRepository {
    Task AddAsync(Product item);
    Task DeleteAsync(string id);
    Task<Product> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetMultipleAsync(Expression<Func<Product, bool>> filter, int elements);
    Task UpdateAsync(Product item);
    Task PatchAsync(string id, Product item);
}