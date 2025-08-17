using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Idempotencia
{
    [Key]
    public Guid Chave_idempotencia { get; set; } = Guid.NewGuid();
    public string requisicao { get; set; } = default!;
    public string resultado { get; set; } = default!;
}