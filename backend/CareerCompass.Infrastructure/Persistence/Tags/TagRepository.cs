using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence.Tags;

internal class TagRepository(AppDbContext dbContext) : ITagRepository
{
    public Task<bool> Exists(TagId id, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .AsNoTracking()
            .AnyAsync(f => f.Id.ToString() == id, cancellationToken);
    }

    public Task<bool> Exists(UserId id, string name, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .AsNoTracking()
            .Where(f => f.Agent.Id.ToString() == id)
            .Where(f => f.Name == name)
            .AnyAsync(cancellationToken);
    }

    public async Task<Tag> Create(Tag field, CancellationToken cancellationToken)
    {
        var agent = await dbContext.Agents.FirstAsync(a => a.Id.ToString() == field.UserId, cancellationToken);

        var fieldTable = new TagTable
        {
            Id = Guid.Parse(field.Id),
            Name = field.Name,
            Agent = agent,
        };

        var result = await dbContext.Tags.AddAsync(fieldTable);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapTag(result.Entity);
    }

    public Task<Tag> Get(TagId id, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .Where(f => f.Id.ToString() == id)
            .AsNoTracking()
            .Select(f => new Tag(f.Id, f.Name, f.Agent.Id.ToString()))
            .FirstAsync(cancellationToken);
    }

    private Tag MapTag(TagTable tagTable)
    {
        return new Tag(tagTable.Id, tagTable.Name, tagTable.Agent.Id.ToString());
    }
}