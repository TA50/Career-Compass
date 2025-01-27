using CareerCompass.Application.Users;

namespace CareerCompass.Application.Tags;

public interface ITagRepository
{
    public Task<bool> Exists(TagId id, CancellationToken cancellationToken);

    public Task<bool> Exists(UserId userId, string name, CancellationToken cancellationToken);
    public Task<Tag> Get(TagId id, CancellationToken cancellationToken);

    public Task<Tag> Create(Tag tag, CancellationToken cancellationToken);
}