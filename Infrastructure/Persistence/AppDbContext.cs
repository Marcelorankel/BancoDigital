using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ContaCorrente> ContaCorrente { get; set; } = null!;
        public DbSet<Movimento> Movimento { get; set; } = null!;
        public DbSet<Transferencia> Transferencia { get; set; } = null!;
        public DbSet<Idempotencia> Idempotencia { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ContaCorrente
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.HasKey(c => c.IdContaCorrente);
                entity.Property(m => m.IdContaCorrente)
                  .HasColumnType("char(36)");
            });

            // Movimento
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.HasKey(m => m.Idmovimento);
                entity.Property(m => m.Idmovimento)
                .HasColumnType("char(36)");
                entity.Property(m => m.IdContaCorrente)
                  .HasColumnType("char(36)");
                // Mapeamento da FK 1:N com ContaCorrente
                entity.HasOne(m => m.ContaCorrente)
                      .WithMany(c => c.Movimentos)
                      .HasForeignKey(m => m.IdContaCorrente)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Transferencia
            modelBuilder.Entity<Transferencia>(entity =>
            {
                entity.HasKey(c => c.IdTransferencia);
                entity.Property(m => m.IdTransferencia)
                  .HasColumnType("char(36)");
            });

            // Idempotencia
            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.HasKey(c => c.Chave_idempotencia);
                entity.Property(m => m.Chave_idempotencia)
                  .HasColumnType("char(36)");
            });
        }
    }
}