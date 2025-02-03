using CareerCompass.Application.Users;

namespace CareerCompass.Application.Fields;

public interface IFieldRepository
{
    public Task<bool> Exists(FieldId id, CancellationToken cancellationToken);
    public Task<bool> Exists(UserId id, string name, CancellationToken cancellationToken);

    public Task<Field> Create(Field field, CancellationToken cancellationToken);
    public Task<Field?> Get(UserId userId, FieldId id, CancellationToken cancellationToken);

    public Task<Field?> Get(UserId userId, FieldId id)
    {
        return Get(userId, id, CancellationToken.None);
    }

    public Task<IList<Field>> Get(UserId id, CancellationToken cancellationToken);
    public Task<IList<Field>> Get(UserId id);
}