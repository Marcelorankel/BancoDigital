using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly AppDbContext _db;

        public IdempotenciaRepository(AppDbContext db) => _db = db;
        public async Task<Idempotencia> AddTransferenciaAsync(Idempotencia idempotencia, CancellationToken ct = default)
        {
            _db.Idempotencia.Add(idempotencia);
            await _db.SaveChangesAsync(ct);
            return idempotencia;
        }
    }
}