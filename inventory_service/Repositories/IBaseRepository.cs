using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_service.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T?>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}