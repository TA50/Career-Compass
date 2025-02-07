using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using CareerCompass.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence.Users;

/// <summary>
/// User is a domain entity and is represented by Agent Table
/// </summary>
/// <param name="dbContext"></param>
internal class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<bool> Exists(UserId id, CancellationToken cancellationToken)
    {
        return dbContext.Agents.AnyAsync(a => a.Id.ToString() == id, cancellationToken);
    }

    public async Task<User?> Get(UserId id, CancellationToken cancellationToken)
    {
        var result = await dbContext.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Include(a => a.Fields)
            .Include(a => a.Scenarios)
            .FirstOrDefaultAsync(a => a.Id.ToString() == id, cancellationToken);

        if (result is null)
        {
            return null;
        }

        return MapToUser(result);
    }

    public async Task<User?> GetFromIdentity(string id, CancellationToken cancellationToken)
    {
        var result = await dbContext.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Include(a => a.Fields)
            .Include(a => a.Scenarios)
            .FirstOrDefaultAsync(a => a.User.Id == id, cancellationToken);

        if (result is null)
        {
            return null;
        }

        return MapToUser(result);
    }

    public async Task<User> Update(User user, CancellationToken? cancellationToken = null)
    {
        var agentEntity = await dbContext.Agents
            .FirstAsync(x => x.Id.ToString() == user.Id,
                cancellationToken ?? CancellationToken.None);

        agentEntity.FirstName = user.FirstName;
        agentEntity.LastName = user.LastName;

        await dbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
        var result = await Get(new UserId(agentEntity.Id), cancellationToken ?? CancellationToken.None);
        if (result is null)
        {
            throw new DatabaseOperationException("User not found");
        }

        return result;
    }


    private static User MapToUser(AgentTable agent)
    {
        return new User(
            id: agent.Id.ToString(),
            email: agent.User.Email ?? agent.User.UserName ?? "",
            firstName: agent.FirstName,
            lastName: agent.LastName,
            tagIds: agent.Tags.Select(t => (TagId)t.Id).ToList(),
            fieldIds: agent.Fields.Select(f => new FieldId(f.Id)).ToList(),
            scenarioIds: agent.Scenarios
                .Select(f => new ScenarioId(f.Id))
                .ToList()
        );
    }
}