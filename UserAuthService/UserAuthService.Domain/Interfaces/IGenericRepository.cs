using System.Linq.Expressions;
using UserAuthService.Domain.Common;

namespace UserAuthService.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}
