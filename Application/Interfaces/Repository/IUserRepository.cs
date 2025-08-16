using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<User> AddAsync(User user, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
}