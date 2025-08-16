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
    public double Saldo { get; set; } = default!;
    public string Cpf { get; set; } = default!;
}