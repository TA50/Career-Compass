using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Abstractions;

public interface IUserRepository : IRepository<User, UserId>
{
}