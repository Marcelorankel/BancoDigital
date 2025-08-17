using Application.Interfaces.Service;
using Domain.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [HttpPost("Cadastrar")]
    public async Task<IActionResult> Create(UsuarioRequest usuario, CancellationToken ct)
    {
        Guid newGuid = Guid.NewGuid();
        var created = await _contaCorrenteService.AddContaAsync(usuario, ct);
        return CreatedAtAction(nameof(Create), new { userName = created.Nome }, created);
    }

    [Authorize]
    [HttpPost("AlterarStatusConta")]
    public async Task<IActionResult> AlterarStatusConta(StatusContaCorrenteRequest request, CancellationToken ct)
    {
        var res = await _contaCorrenteService.AlterarStatusContaAsync(request, User.Identity?.Name, ct);
        return StatusCode(204, res);
    }
    
    [Authorize]
    [HttpPost("Movimento")]
    public async Task<IActionResult> Movimento(MovimentacaoContaCorrenteRequest request, CancellationToken ct)
    {
        var res = await _contaCorrenteService.MovimentoContaAsync(request, User.Identity?.Name, ct);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("ObterSaldoContaCorrente")]
    public async Task<IActionResult> ObterSaldoContaCorrente(int numeroConta, CancellationToken ct)
    {
        var res = await _contaCorrenteService.ObterSaldoContaAsync(numeroConta, User.Identity?.Name, ct);
        return Ok(res);
    }

    [Authorize]
    [HttpPost("RegistrarDebito")]
    public async Task<IActionResult> RegistrarDebito(OperacaoRequest request, CancellationToken ct)
    {
        var conta = await _contaCorrenteService.RegistrarDebito(request);
        return Ok(conta);
    }

    [Authorize]
    [HttpPost("RegistrarCredito")]
    public async Task<IActionResult> RegistrarCredito(OperacaoRequest request, CancellationToken ct)
    {
        var conta = await _contaCorrenteService.RegistrarCredito(request);
        return Ok(conta);
    }
}