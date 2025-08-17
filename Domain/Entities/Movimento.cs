using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Movimento
{
    [Key]
    public Guid Idmovimento { get; set; } = Guid.NewGuid();
    public Guid IdContaCorrente { get; set; }
    public ContaCorrente ContaCorrente { get; set; } = default!;
    public DateTime DataMovimento { get; set; }
    public string TipoMovimento { get; set; } = default!;
    public decimal Valor { get; set; } = default!;
}