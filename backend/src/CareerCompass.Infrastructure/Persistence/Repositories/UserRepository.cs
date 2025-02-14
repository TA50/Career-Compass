using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

internal class UserRepository(AppDbContext dbContext) : RepositoryBase<User, UserId>(dbContext), IUserRepository
{
}