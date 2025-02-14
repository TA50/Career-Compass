using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Common.Specifications;

namespace CareerCompass.Core.Common.Abstractions.Repositories;

public struct RepositoryResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    public RepositoryResult()
    {
        IsSuccess = true;
        ErrorMessage = null;
    }

    public RepositoryResult(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }
}

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : ValueObject
{
    public Task<TEntity?> Get(TId id,
        bool trackChanges,
        CancellationToken? cancellationToken = null);

    public Task<TEntity?> Get(TId id,
        CancellationToken? cancellationToken = null);

    public Task<IList<TEntity>> Get(
        ISpecification<TEntity, TId> specification,
        bool trackChanges,
        CancellationToken? cancellationToken = null);

    public Task<IList<TEntity>> Get(
        ISpecification<TEntity, TId> specification,
        CancellationToken? cancellationToken = null);

    public Task<TEntity?> Single(
        ISpecification<TEntity, TId> specification,
        bool trackChanges,
        CancellationToken? cancellationToken = null
    );

    public Task<TEntity?> Single(
        ISpecification<TEntity, TId> specification,
        CancellationToken? cancellationToken = null
    );

    public Task<bool> Exists(
        ISpecification<TEntity, TId> specification,
        CancellationToken? cancellationToken = null);

    public Task<bool> Exists(
        TId id,
        CancellationToken? cancellationToken = null);

    /// <summary>
    /// Create a new entity in the database
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>RepositoryResult</returns>
    public Task<RepositoryResult> Create(TEntity entity,
        CancellationToken? cancellationToken = null);

    /// <summary>
    ///   Save changes to the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>RepositoryResult</returns>
    public Task<RepositoryResult> Save(CancellationToken? cancellationToken = null);
}