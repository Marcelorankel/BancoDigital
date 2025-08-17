using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Transferencia
{
    [Key]
    public Guid IdTransferencia { get; set; } = Guid.NewGuid();
    public Guid IdContaCorrente_Origem { get; set; }
    public Guid IdContaCorrente_Destino { get; set; }
    public DateTime DataMovimento { get; set; } = default!;
    public decimal valor { get; set; } = default!;
}