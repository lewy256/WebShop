using System.Linq.Expressions;

namespace ProductApi.Interfaces;

public interface IRepositoryBase<T> where T : class {
    public Task AddAsync(T item);
    public Task DeleteAsync(string id);
    public Task<T> GetByIdAsync(T item, string id);
    public Task<IEnumerable<T>> GetMultipleAsync(Expression<Func<T, bool>> filter);
    public Task UpdateAsync(T item);
    public Task PatchAsync(string id, T item);
}