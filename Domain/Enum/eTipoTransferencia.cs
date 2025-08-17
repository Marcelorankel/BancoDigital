using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
