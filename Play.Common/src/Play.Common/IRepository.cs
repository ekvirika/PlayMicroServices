using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetOneAsync(Guid id);
        Task<T> GetOneAsync(Expression<Func<T, bool>> filter);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(Guid id);

    }
}