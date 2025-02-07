namespace CareerCompass.Application.Users;

public interface IUserRepository
{
    Task<bool> Exists(UserId id, CancellationToken cancellationToken);


    Task<User?> Get(UserId id, CancellationToken cancellationToken);

    Task<User?> GetFromIdentity(string id, CancellationToken cancellationToken);

    Task<User> Update(User user, CancellationToken? cancellationToken = null);
}