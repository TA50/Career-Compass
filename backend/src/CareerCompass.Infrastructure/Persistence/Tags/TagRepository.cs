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


    public Task<IList<Tag>> Get(UserId id)
    {
        return Get(id, CancellationToken.None);
    }

    public async Task<IList<Tag>> Get(UserId id, CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .AsNoTracking()
            .Where(f => f.Agent.Id.ToString() == id)
            .Select(x => MapTag(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag> Create(Tag tag, CancellationToken cancellationToken)
    {
        var agent = await dbContext.Agents.FirstAsync(a => a.Id.ToString() == tag.UserId, cancellationToken);

        var tagTable = new TagTable
        {
            Id = Guid.Parse(tag.Id),
            Name = tag.Name,
            Agent = agent,
        };

        var result = await dbContext.Tags.AddAsync(tagTable);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapTag(result.Entity);
    }


    public Task<Tag?> Get(UserId userId, TagId id, CancellationToken cancellationToken)
    {
        return dbContext.Tags
            .Where(f => f.Id.ToString() == id)
            .Where(f => f.Agent.Id.ToString() == userId)
            .AsNoTracking()
            .Select(f => new Tag(f.Id, f.Name, f.Agent.Id.ToString()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Tag MapTag(TagTable tagTable)
    {
        return new Tag(tagTable.Id, tagTable.Name, tagTable.Agent.Id.ToString());
    }
}