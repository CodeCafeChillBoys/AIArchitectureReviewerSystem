using Microsoft.EntityFrameworkCore;
using UserAuthService.Domain.Interfaces;
using UserAuthService.Infrastructure.Persistence;

namespace UserAuthService.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly UserAuthDbContext DbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(UserAuthDbContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
