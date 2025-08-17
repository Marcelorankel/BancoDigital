using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IMovimentoRepository
{
    Task<Movimento?> GetMovimentoByIdAsync(Guid idMovimento, CancellationToken ct = default);
    Task<List<Movimento>> ObterMovimentosPorIdContaCorrenteAsync(Guid idContaCorrente);
    Task<Movimento> AddMovimentoAsync(Movimento movimento, CancellationToken ct = default);
}