using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly AppDbContext _db;

        public TransferenciaRepository(AppDbContext db) => _db = db;
        public async Task<Transferencia> AddTransferenciaAsync(Transferencia transferencia, CancellationToken ct = default)
        {
            _db.Transferencia.Add(transferencia);
            await _db.SaveChangesAsync(ct);
            return transferencia;
        }
    }
}