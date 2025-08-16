using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        => await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
        => await db.Users.AnyAsync(u => u.UserName == userName, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }
}
