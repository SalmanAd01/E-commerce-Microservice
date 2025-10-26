using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly string _tableName;

        public BaseRepository(DbContext context, string tableName)
        {
            _context = context;
            _tableName = tableName;
        }

        public async Task<T> CreateAsync(T entity, System.Threading.CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(int id, System.Threading.CancellationToken cancellationToken = default)
        {
            var set = _context.Set<T>();
            var entity = await set.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
            if (entity != null)
            {
                set.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<List<T>> GetAllAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(int id, System.Threading.CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<T?> UpdateAsync(T entity, System.Threading.CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }
    }
}
