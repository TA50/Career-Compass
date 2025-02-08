using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Users;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase<User, UserId>, IUserRepository
{
}