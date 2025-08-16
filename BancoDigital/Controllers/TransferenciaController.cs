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
public class TransferenciaController : ControllerBase
{
    private readonly IContaCorrenteService _contaCorrenteService;

    public TransferenciaController(IContaCorrenteService contaCorrenteService)
    {
        _contaCorrenteService = contaCorrenteService;
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