using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Tags;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

public class TagRepository : RepositoryBase<Tag, TagId>, ITagRepository
{
}