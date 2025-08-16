using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Request
{
    public class TransferenciaRequest
    {
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public double valor {  get; set; }
    }
}