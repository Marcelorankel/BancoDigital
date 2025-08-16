using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly AppDbContext _db;

        public ContaCorrenteRepository(AppDbContext db) => _db = db;

        public async Task<ContaCorrente?> GetContaByNameAsync(string userName, CancellationToken ct = default)
        {
            return await _db.ContaCorrente.AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Nome == userName, ct);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
        {
            return await _db.ContaCorrente.AnyAsync(u => u.Nome == userName, ct);
        }

        public async Task<ContaCorrente> AddContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default)
        {
            _db.ContaCorrente.Add(contaCorrente);
            await _db.SaveChangesAsync(ct);
            return contaCorrente;
        }
        public async Task<ContaCorrente?> GetUltimoNumeroContaValido(CancellationToken ct = default)
        {
            var ultimaConta = await _db.ContaCorrente
                               .AsNoTracking()
                               .OrderByDescending(u => u.Numero)
                               .FirstOrDefaultAsync(ct);
            return ultimaConta;
        }

        public async Task<ContaCorrente?> UpdateContaAsync(ContaCorrente contaCorrente, CancellationToken ct = default)
        {
            _db.ContaCorrente.Update(contaCorrente);
            await _db.SaveChangesAsync(ct);
            return contaCorrente;
        }

        public async Task<ContaCorrente?> GetContaByNumeroConta(int numeroConta, CancellationToken ct = default)
        {
            try
            {
                return await _db.ContaCorrente
                                     .FirstOrDefaultAsync(u => u.Numero == numeroConta, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                throw;
            }
        }

        public async Task<ContaCorrente?> GetContaByCpf(string cpf, CancellationToken ct = default)
        {
            try
            {
                return await _db.ContaCorrente
                                     .FirstOrDefaultAsync(u => u.Cpf == cpf, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                throw;
            }
        }
    }
}