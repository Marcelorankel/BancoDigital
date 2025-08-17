using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly AppDbContext _db;

        public MovimentoRepository(AppDbContext db) => _db = db;

        public async Task<Movimento?> GetMovimentoByIdAsync(Guid idMovimento, CancellationToken ct = default)
        {
            return await _db.Movimento.AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Idmovimento == idMovimento, ct);
        }

        public async Task<List<Movimento>> ObterMovimentosPorIdContaCorrenteAsync(Guid idContaCorrente)
        {
            return await _db.Movimento
                                 .Where(m => m.IdContaCorrente == idContaCorrente)
                                 .OrderByDescending(m => m.DataMovimento)
                                 .ToListAsync();
        }

        public async Task<Movimento> AddMovimentoAsync(Movimento movimento, CancellationToken ct = default)
        {
            _db.Movimento.Add(movimento);
            await _db.SaveChangesAsync(ct);
            return movimento;
        }
    }
}