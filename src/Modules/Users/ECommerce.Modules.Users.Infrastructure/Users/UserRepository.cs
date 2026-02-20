using ECommerce.Modules.Users.Domain.Users;
using ECommerce.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext dbContext) : IUserRepository
{
    public Task<User?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public void Insert(User user)
    {
        dbContext.Users.Add(user);
    }
}
