using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Common.Specifications;

public interface ISpecification<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : ValueObject
{
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}