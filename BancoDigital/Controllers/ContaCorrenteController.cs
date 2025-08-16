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
public class ContaCorrenteController : ControllerBase
{
    private readonly IContaCorrenteService _contaCorrenteService;

    public ContaCorrenteController(IContaCorrenteService contaCorrenteService)
    {
        _contaCorrenteService = contaCorrenteService;
    }

   
    [AllowAnonymous]
    [HttpPost("Cadastro")]
    public async Task<IActionResult> Create(UsuarioRequest usuario, CancellationToken ct)
    {
        var created = await _contaCorrenteService.AddContaAsync(usuario, ct);
        return CreatedAtAction(nameof(Create), new { userName = created.Nome }, created);
    }
}

