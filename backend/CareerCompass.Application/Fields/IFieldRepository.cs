using CareerCompass.Application.Users;

namespace CareerCompass.Application.Fields;

public interface IFieldRepository
{
    public Task<bool> Exists(FieldId id, CancellationToken cancellationToken);
    public Task<bool> Exists(UserId id, string name, CancellationToken cancellationToken);

    public Task<Field> Create(Field field, CancellationToken cancellationToken);
    public Task<Field> Get(FieldId id, CancellationToken cancellationToken);
}