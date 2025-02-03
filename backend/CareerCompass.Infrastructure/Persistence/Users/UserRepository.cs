using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence.Users;

// User is a domain entity and is represented by Agent Table
internal class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<bool> Exists(UserId id, CancellationToken cancellationToken)
    {
        return dbContext.Agents.AnyAsync(a => a.Id.ToString() == id, cancellationToken);
    }

    public async Task<User> Get(UserId id, CancellationToken cancellationToken)
    {
        var result = await dbContext.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Include(a => a.Fields)
            .Include(a => a.Scenarios)
            .FirstAsync(a => a.Id.ToString() == id, cancellationToken);

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
            .FirstAsync(a => a.User.Id == id, cancellationToken);

        return MapToUser(result);
    }


    private User MapToUser(AgentTable agent)
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