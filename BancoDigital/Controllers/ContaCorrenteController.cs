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
using static System.Net.Mime.MediaTypeNames;

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
        Guid newGuid = Guid.NewGuid();
        var created = await _contaCorrenteService.AddContaAsync(usuario, ct);
        return CreatedAtAction(nameof(Create), new { userName = created.Nome }, created);
    }

    [HttpPost("Transferencia")]
    public async Task<IActionResult> Transferencia(TransferenciaRequest transferencia, CancellationToken ct)
    {
        var conta = await _contaCorrenteService.TransferenciaContaAsync(transferencia);
        if (conta is null) return NotFound();
        return Ok(conta);
    }

    //[Authorize]
    [AllowAnonymous]
    [HttpPost("AlterarStatusConta")]
    public async Task<IActionResult> AlterarStatusConta(StatusContaCorrenteRequest request, CancellationToken ct)
    {
        var res = await _contaCorrenteService.AlterarStatusContaAsync(request, User.Identity?.Name, ct);
        return StatusCode(204, res);
    }
    [AllowAnonymous]
    [HttpPost("Movimento")]
    public async Task<IActionResult> Movimento(MovimentacaoContaCorrenteRequest request, CancellationToken ct)
    {
        var res = await _contaCorrenteService.MovimentoContaAsync(request, User.Identity?.Name, ct);
        return NoContent();
    }
    [AllowAnonymous]
    [HttpPost("ObterSaldoContaCorrente")]
    public async Task<IActionResult> ObterSaldoContaCorrente(int numeroConta, CancellationToken ct)
    {
        var res = await _contaCorrenteService.ObterSaldoContaAsync(numeroConta, User.Identity?.Name, ct);
        return Ok(res);
    }
}

