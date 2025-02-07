namespace CareerCompass.Application.Users;

public interface IUserRepository
{
    public Task<bool> Exists(UserId id, CancellationToken cancellationToken);


    public Task<User> Get(UserId id, CancellationToken cancellationToken);

    public Task<User?> GetFromIdentity(string id, CancellationToken cancellationToken);
}