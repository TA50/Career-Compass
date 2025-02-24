using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Common.Specifications;

namespace CareerCompass.Core.Common.Abstractions.Repositories;

public struct RepositoryResult
{
    private bool _isSuccess = false;
    private string? _errorMessage;
    public bool IsSuccess => _isSuccess;

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (value == null) return;

            _errorMessage = value;
            _isSuccess = true;
        }
    }

    public RepositoryResult()
    {
        ErrorMessage = null;
        _isSuccess = true;
    }

    public RepositoryResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        _isSuccess = false;
    }
}

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : ValueObject
{
    public Task StartTransaction(CancellationToken? cancellationToken = null);

    public Task CommitTransaction(CancellationToken? cancellationToken = null);

    public Task RollbackTransaction(CancellationToken? cancellationToken = null);

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

    public Task<RepositoryResult> Delete(TId id,
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