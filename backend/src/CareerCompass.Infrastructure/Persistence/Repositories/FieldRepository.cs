using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Fields;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

internal class FieldRepository(AppDbContext dbContext) : RepositoryBase<Field, FieldId>(dbContext), IFieldRepository
{
}