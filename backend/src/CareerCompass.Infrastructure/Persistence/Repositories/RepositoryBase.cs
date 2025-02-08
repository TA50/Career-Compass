using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Common.Specifications;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    
    private readonly AppDbContext _dbContext;
    public Task<TEntity?> Get(TId id, bool trackChanges, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> Get(TId id, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<IList<TEntity>> Get(ISpecification<TEntity, TId> specification, bool trackChanges, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<IList<TEntity>> Get(ISpecification<TEntity, TId> specification, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> Single(ISpecification<TEntity, TId> specification, bool trackChanges, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> Single(ISpecification<TEntity, TId> specification, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(ISpecification<TEntity, TId> specification, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(TId id, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<RepositoryResult> Create(TEntity entity, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<RepositoryResult> Save(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}