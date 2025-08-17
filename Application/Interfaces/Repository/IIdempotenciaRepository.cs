using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IIdempotenciaRepository
{
    Task<Idempotencia> AddTransferenciaAsync(Idempotencia transferencia, CancellationToken ct = default);
}