using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface ITransferenciaRepository
{
    Task<Transferencia> AddTransferenciaAsync(Transferencia transferencia, CancellationToken ct = default);
}