using MaterialWarehouse.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaterialWarehouse.DAL.Repositories;

public class Repository<T>(MaterialWarehouseDbContext context) : IRepository<T> where T : class
{
    protected readonly MaterialWarehouseDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);
}
