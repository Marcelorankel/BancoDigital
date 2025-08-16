namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
}