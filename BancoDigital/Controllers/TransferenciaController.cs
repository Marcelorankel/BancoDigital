using Application.Interfaces.Service;
using Domain.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransferenciaController : ControllerBase
{
    private readonly ITransferenciaService _transferenciaService;

    public TransferenciaController(ITransferenciaService transferenciaService)
    {
        _transferenciaService = transferenciaService;
    }

    [Authorize]
    [HttpPost("Transferir")]
    public async Task<IActionResult> Transferir(TransferenciaRequest transferencia, CancellationToken ct)
    {
        var token = HttpContext.Request.Headers["Authorization"]
           .ToString().Replace("Bearer ", "");
        var res = await _transferenciaService.TransferenciaContaAsync(transferencia, token);
        return StatusCode(204, res);
    }
}