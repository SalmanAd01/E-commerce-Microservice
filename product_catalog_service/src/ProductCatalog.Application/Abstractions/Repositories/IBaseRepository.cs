using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.Application.Abstractions.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
