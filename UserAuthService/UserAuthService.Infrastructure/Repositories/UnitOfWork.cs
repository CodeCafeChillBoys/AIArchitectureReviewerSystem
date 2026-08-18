using System.Collections.Concurrent;
using UserAuthService.Domain.Interfaces;
using UserAuthService.Infrastructure.Persistence;

namespace UserAuthService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserAuthDbContext _dbContext;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(UserAuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return (IGenericRepository<T>)_repositories.GetOrAdd(
            typeof(T),
            _ => new GenericRepository<T>(_dbContext));
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
