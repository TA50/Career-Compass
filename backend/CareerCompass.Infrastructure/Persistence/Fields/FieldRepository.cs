using CareerCompass.Application.Fields;
using CareerCompass.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence.Fields;

internal class FieldRepository(AppDbContext dbContext) : IFieldRepository
{
    private AppDbContext dbContext = dbContext;

    public Task<bool> Exists(FieldId id, CancellationToken cancellationToken)
    {
        return dbContext.Fields
            .AsNoTracking()
            .AnyAsync(f => f.Id == id, cancellationToken);
    }

    public Task<bool> Exists(UserId id, string name, CancellationToken cancellationToken)
    {
        return dbContext.Fields
            .AsNoTracking()
            .Where(f => f.Agent.Id == id)
            .Where(f => f.Name == name)
            .AnyAsync(cancellationToken);
    }

    public async Task<Field> Create(Field field, CancellationToken cancellationToken)
    {
        var agent = await dbContext.Agents.FirstAsync(a => a.Id == field.UserId, cancellationToken);

        var fieldTable = new FieldTable
        {
            Id = field.Id,
            Name = field.Name,
            Agent = agent,
        };

        var result = await dbContext.Fields.AddAsync(fieldTable);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new Field(result.Entity.Id,
            result.Entity.Name,
            result.Entity.Agent.Id);
    }

    public Task<Field> Get(FieldId id, CancellationToken cancellationToken)
    {
        return dbContext.Fields
            .Where(f => f.Id == id)
            .AsNoTracking()
            .Select(f => new Field(f.Id, f.Name, f.Agent.Id))
            .FirstAsync(cancellationToken);
    }
}