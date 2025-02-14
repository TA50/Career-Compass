using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}