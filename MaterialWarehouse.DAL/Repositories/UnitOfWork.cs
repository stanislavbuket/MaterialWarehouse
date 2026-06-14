using MaterialWarehouse.DAL.Interfaces;
using System.Collections.Concurrent;

namespace MaterialWarehouse.DAL.Repositories;

public class UnitOfWork(MaterialWarehouseDbContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IRepository<T> GetRepository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(context));
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

    public void Dispose() => context.Dispose();
}
