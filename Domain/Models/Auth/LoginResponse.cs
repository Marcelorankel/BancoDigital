namespace Application.Models.Auth;
public class LoginRequest
{
    public int NumeroConta { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public string Password { get; set; } = default!;
}