namespace UserAuthService.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
