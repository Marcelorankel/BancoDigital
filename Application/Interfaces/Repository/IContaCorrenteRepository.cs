using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IContaCorrenteRepository
{
    Task<ContaCorrente?> GetContaByNameAsync(string userName, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    Task<ContaCorrente> AddContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default);
    Task<ContaCorrente?> GetUltimoNumeroContaValido(CancellationToken ct = default);
    Task<ContaCorrente?> GetContaByNumeroConta(int numeroConta, CancellationToken ct = default);
    Task<ContaCorrente> UpdateContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default);
    Task<ContaCorrente?> GetContaByCpf(string cpf, CancellationToken ct = default);
}