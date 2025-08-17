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
        //public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
        //{
        //    return await _db.ContaCorrente.AnyAsync(u => u.Nome == userName, ct);
        //}

        public async Task<Movimento> AddMovimentoAsync(Movimento movimento, CancellationToken ct = default)
        {
            _db.Movimento.Add(movimento);
            await _db.SaveChangesAsync(ct);
            return movimento;
        }
        //public async Task<ContaCorrente?> GetUltimoNumeroContaValido(CancellationToken ct = default)
        //{
        //    var ultimaConta = await _db.ContaCorrente
        //                       .AsNoTracking()
        //                       .OrderByDescending(u => u.Numero)
        //                       .FirstOrDefaultAsync(ct);
        //    return ultimaConta;
        //}

        //public async Task<ContaCorrente> UpdateContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default)
        //{
        //    _db.ContaCorrente.Update(contaCorrente);
        //    await _db.SaveChangesAsync(ct);
        //    return contaCorrente;
        //}

        //public async Task<ContaCorrente?> GetContaByNumeroConta(int numeroConta, CancellationToken ct = default)
        //{
        //    try
        //    {
        //        return await _db.ContaCorrente
        //                             .FirstOrDefaultAsync(u => u.Numero == numeroConta, ct);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        //        throw;
        //    }
        //}

        //public async Task<ContaCorrente?> GetContaByCpf(string cpf, CancellationToken ct = default)
        //{
        //    try
        //    {
        //        return await _db.ContaCorrente
        //                             .FirstOrDefaultAsync(u => u.Cpf == cpf, ct);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        //        throw;
        //    }
        //}
    }
}