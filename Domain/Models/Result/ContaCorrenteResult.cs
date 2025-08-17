using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Models.Request
{
    public class ContaCorrenteResult
    {
        public int NumeroConta { get; set; }
        public string NomeTitular { get; set; } = default!;
        public DateTime DtHraConsulta { get; set; }
        public string SaldoAtual { get; set; }
    }
}