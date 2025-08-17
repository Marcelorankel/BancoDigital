using Application.Models.Auth;

namespace Application.Interfaces.Service;

public interface IAuthService
{
    string Login(LoginRequest loginRequest);
}