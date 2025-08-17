using Domain.Entities;
using Domain.Models.Request;

namespace Application.Interfaces.Service;

public interface IContaCorrenteService
{
    Task<ContaCorrente?> GetContaByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    Task<ContaCorrente> AddContaAsync(UsuarioRequest usuario, CancellationToken ct = default);
    Task<ContaCorrente> AlterarStatusContaAsync(StatusContaCorrenteRequest request, string numeroContaToken, CancellationToken ct = default);
    Task<Movimento> MovimentoContaAsync(MovimentacaoContaCorrenteRequest request, string numeroContaToken, CancellationToken ct = default);
    Task<ContaCorrenteResult>ObterSaldoContaAsync(int numeroConta, string numeroContaToken, CancellationToken ct = default);
    Task<ContaCorrente> RegistrarDebito(OperacaoRequest request, CancellationToken ct = default);
    Task<ContaCorrente> RegistrarCredito(OperacaoRequest request, CancellationToken ct = default);
}