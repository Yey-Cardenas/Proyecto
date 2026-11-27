using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data.Interfaces;

namespace Proyecto.Data;

public sealed class Repository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    public Task<TEntity?> GetByIdAsync(
        CancellationToken cancellationToken,
        params object[] keyValues) =>
        context.Set<TEntity>().FindAsync(keyValues, cancellationToken).AsTask();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    public Task<List<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().CountAsync(predicate, cancellationToken);

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}
