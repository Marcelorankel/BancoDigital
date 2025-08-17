using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ContaCorrente
{
    [Key]
    public Guid IdContaCorrente { get; set; } = Guid.NewGuid();
    public int Numero { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public int Ativo { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public string Salt { get; set; } = default!;
    public decimal Saldo { get; set; } = default!;
    public string Cpf { get; set; } = default!;

    public ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();
}