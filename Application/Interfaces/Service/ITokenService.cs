using Domain.Entities;

namespace Application.Interfaces.Service;

public interface ITokenService
{
    string GenerateToken(User user);
}