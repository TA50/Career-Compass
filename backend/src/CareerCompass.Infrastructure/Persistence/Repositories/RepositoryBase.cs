using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Common.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

internal abstract class RepositoryBase<TEntity, TId>(AppDbContext dbContext)
    : IRepository<TEntity, TId>, IDisposable, IAsyncDisposable where TEntity : Entity<TId>
    where TId : ValueObject
{
    protected IDbContextTransaction? Transaction;

    public async Task StartTransaction(CancellationToken? cancellationToken = null)
    {
        Transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task CommitTransaction(CancellationToken? cancellationToken = null)
    {
        return Transaction?.CommitAsync(cancellationToken ?? CancellationToken.None) ?? Task.CompletedTask;
    }

    public Task RollbackTransaction(CancellationToken? cancellationToken = null)
    {
        return dbContext.Database.RollbackTransactionAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task<TEntity?> Get(TId id, bool trackChanges, CancellationToken? cancellationToken = null)
    {
        var set = GetSet(trackChanges);
        return set.FirstOrDefaultAsync(e => e.Id == id,
            cancellationToken ?? CancellationToken.None);
    }

    public Task<TEntity?> Get(TId id, CancellationToken? cancellationToken = null)
    {
        return Get(id, false, cancellationToken);
    }

    public async Task<IList<TEntity>> Get(ISpecification<TEntity, TId> specification, bool trackChanges,
        CancellationToken? cancellationToken = null)
    {
        var set = GetSet(trackChanges);
        return await specification.Apply(set)
            .ToListAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task<IList<TEntity>> Get(ISpecification<TEntity, TId> specification,
        CancellationToken? cancellationToken = null)
    {
        return Get(specification, false, cancellationToken);
    }

    public Task<TEntity?> Single(ISpecification<TEntity, TId> specification, bool trackChanges,
        CancellationToken? cancellationToken = null)
    {
        var set = GetSet(trackChanges);
        return specification.Apply(set)
            .FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task<TEntity?> Single(ISpecification<TEntity, TId> specification,
        CancellationToken? cancellationToken = null)
    {
        return Single(specification, false, cancellationToken);
    }

    public Task<bool> Exists(ISpecification<TEntity, TId> specification, CancellationToken? cancellationToken = null)
    {
        var set = GetSet(false);
        return specification.Apply(set)
            .AnyAsync(cancellationToken ?? CancellationToken.None);
    }

    public Task<bool> Exists(TId id, CancellationToken? cancellationToken = null)
    {
        var set = GetSet(false);
        return set.AnyAsync(e => e.Id == id, cancellationToken ?? CancellationToken.None);
    }

    public async Task<RepositoryResult> Delete(TId id, CancellationToken? cancellationToken = null)
    {
        try
        {
            await dbContext.Set<TEntity>().Where(t => t.Id == id)
                .ExecuteDeleteAsync(cancellationToken ?? CancellationToken.None);

            return new RepositoryResult();
        }
        catch (Exception e)
        {
            return new RepositoryResult(e.Message);
        }
    }

    public async Task<RepositoryResult> Create(TEntity entity, CancellationToken? cancellationToken = null)
    {
        try
        {
            await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken ?? CancellationToken.None);
            await dbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
            return new RepositoryResult();
        }
        catch (Exception e)
        {
            return new RepositoryResult(e.Message);
        }
    }

    public async Task<RepositoryResult> Save(CancellationToken? cancellationToken = null)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
            return new RepositoryResult();
        }
        catch (Exception e)
        {
            return new RepositoryResult(e.Message);
        }
    }

    private IQueryable<TEntity> GetSet(bool trackChanges)
    {
        IQueryable<TEntity> set = dbContext.Set<TEntity>();

        if (!trackChanges)
        {
            set = set.AsNoTracking();
        }

        return set;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null) await Transaction.DisposeAsync();
    }
}