using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Idempotencia
{
    [Key]
    public Guid ChaveIdempotencia { get; set; } = Guid.NewGuid();
    public string Requisicao { get; set; } = default!;
    public string Resultado { get; set; } = default!;
}