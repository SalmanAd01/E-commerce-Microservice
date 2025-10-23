using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_service.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
    Task<List<T>> GetAllAsync(System.Threading.CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(int id, System.Threading.CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, System.Threading.CancellationToken cancellationToken = default);
        Task<T?> UpdateAsync(T entity, System.Threading.CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, System.Threading.CancellationToken cancellationToken = default);
    }
}