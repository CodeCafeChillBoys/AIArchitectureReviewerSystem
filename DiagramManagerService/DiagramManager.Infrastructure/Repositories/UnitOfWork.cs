using System.Collections.Concurrent;
using DiagramManager.Domain.Interfaces;
using DiagramManager.Infrastructure.Persistence;

namespace DiagramManager.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DiagramDbContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public UnitOfWork(DiagramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T).Name;
        return (IGenericRepository<T>)_repositories.GetOrAdd(type, _ => new GenericRepository<T>(_dbContext));
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
