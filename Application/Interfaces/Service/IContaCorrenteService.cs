using Domain.Entities;
using Domain.Models.Request;

namespace Application.Interfaces.Service;

public interface IContaCorrenteService
{
    Task<ContaCorrente?> GetContaByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    Task<ContaCorrente> AddContaAsync(UsuarioRequest usuario, CancellationToken ct = default);
    Task<ContaCorrente> TransferenciaContaAsync(TransferenciaRequest contaTransferenciaRequest, CancellationToken ct = default);
}