using Application.Interfaces;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Models.Request;
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
        var x = _contaCorrenteService.GetContaByNameAsync("Marcelo");
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

    //[HttpPost("Cadastro")]
    //[AllowAnonymous]
    //public IActionResult Cadastro([FromBody] LoginRequest request)
    //{
    //    var x = _contaCorrenteService.GetContaByNameAsync("Marcelo");

    //    Guid g = Guid.NewGuid();
    //    // Exemplo simples: username=admin, password=123
    //    if (request.Username == "admin" && request.Password == "123")
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui_super_segura");
    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Username) }),
    //            Expires = DateTime.UtcNow.AddHours(1),
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //        };
    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        return Ok(new { token = tokenHandler.WriteToken(token) });
    //    }
    //    return Unauthorized();
    //}

    [HttpGet("{userName}")]
    public async Task<IActionResult> Get(string userName, CancellationToken ct)
    {
        var conta = await _contaCorrenteService.GetContaByNameAsync(userName);
        if (conta is null) return NotFound();
        return Ok(conta);
    }

    [AllowAnonymous]
    [HttpPost("Cadastro")]
    public async Task<IActionResult> Create(UsuarioRequest usuario, CancellationToken ct)
    {
        var created = await _contaCorrenteService.AddContaAsync(usuario, ct);
        return CreatedAtAction(nameof(Get), new { userName = created.Nome }, created);
    }

    [HttpPost("Transferencia")]
    public async Task<IActionResult> Transferencia(TransferenciaRequest transferencia, CancellationToken ct)
    {
        try
        {
            var conta = await _contaCorrenteService.TransferenciaContaAsync(transferencia);
            if (conta is null) return NotFound();
            return Ok(conta);
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(new { message = ex.Message });
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}