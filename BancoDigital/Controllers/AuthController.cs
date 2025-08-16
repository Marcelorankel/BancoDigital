using Application.Interfaces;
using Application.Interfaces.Service;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IContaCorrenteService _contaCorrenteService;

    public AuthController(IContaCorrenteService contaCorrenteService)
    {
        _contaCorrenteService = contaCorrenteService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var x = _contaCorrenteService.GenerateToken(1);
        // Exemplo simples: username=admin, password=123
        if (request.Username == "admin" && request.Password == "123")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui_super_segura");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
        return Unauthorized();
    }

    [HttpPost("Cadastro")]
    [AllowAnonymous]
    public IActionResult Cadastro([FromBody] LoginRequest request)
    {
        var x = _contaCorrenteService.GenerateToken(1);
        // Exemplo simples: username=admin, password=123
        if (request.Username == "admin" && request.Password == "123")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui_super_segura");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
        return Unauthorized();
    }
}


public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}