using System.ComponentModel;

namespace Domain.Enum
{
    public enum eTipoTransferencia
    {
        [Description("Crédito")]
        Credito = 1,
        [Description("Débito")]
        Debito = 2,
    } 
}