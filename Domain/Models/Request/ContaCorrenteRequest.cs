using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Request
{
    public class ContaCorrenteRequest
    {
        public int NumeroConta { get; set; }
        public string Senha { get; set; } = default!;
    }

    public class StatusContaCorrenteRequest : ContaCorrenteRequest
    {
        public eStatusConta eStatusConta { get; set; }
    }

    public class MovimentacaoContaCorrenteRequest
    {
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string TipoTransferencia { get; set; } = default!;
    }
}