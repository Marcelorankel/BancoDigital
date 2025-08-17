using Domain.Entities;
using Domain.Models.Request;

namespace Application.Interfaces.Service;

public interface ITransferenciaService
{
    Task<Transferencia> TransferenciaContaAsync(TransferenciaRequest contaTransferenciaRequest, string token, CancellationToken ct = default);
}