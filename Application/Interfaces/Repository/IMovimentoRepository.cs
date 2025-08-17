using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IMovimentoRepository
{
    Task<Movimento?> GetMovimentoByIdAsync(Guid idMovimento, CancellationToken ct = default);
    Task<List<Movimento>> ObterMovimentosPorIdContaCorrenteAsync(Guid idContaCorrente);
    Task<Movimento> AddMovimentoAsync(Movimento movimento, CancellationToken ct = default);
    //Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    //Task<ContaCorrente> AddContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default);
    //Task<ContaCorrente?> GetUltimoNumeroContaValido(CancellationToken ct = default);
    //Task<ContaCorrente?> GetContaByNumeroConta(int numeroConta, CancellationToken ct = default);
    //Task<ContaCorrente> UpdateContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default);
    //Task<ContaCorrente?> GetContaByCpf(string cpf, CancellationToken ct = default);
}