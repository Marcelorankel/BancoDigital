namespace Application.Models.Auth;

public record AuthResponse(string AccessToken, string TokenType = "Bearer", int ExpiresIn = 3600);
