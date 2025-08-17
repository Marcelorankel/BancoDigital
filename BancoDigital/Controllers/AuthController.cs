using Application.Interfaces;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Models.Auth;
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
    private readonly IAuthService _authService;
    private readonly IContaCorrenteService _contaCorrenteService;

    public AuthController(IAuthService authService, IContaCorrenteService contaCorrenteService)
    {
        _authService = authService;
        _contaCorrenteService = contaCorrenteService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _authService.Login(request);
        return Ok(new { token = token });
    }

    [HttpGet("{userName}")]
    public async Task<IActionResult> Get(string userName, CancellationToken ct)
    {
        var conta = await _contaCorrenteService.GetContaByNameAsync(userName);
        if (conta is null) return NotFound();
        return Ok(conta);
    }
}