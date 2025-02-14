using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Tags;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

internal class TagRepository(AppDbContext dbContext) : RepositoryBase<Tag, TagId>(dbContext), ITagRepository
{
}