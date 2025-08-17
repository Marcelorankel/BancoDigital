namespace Domain.Models.Request
{
    public class TransferenciaRequest
    {
        public int NumeroContaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor {  get; set; }
    }

    public class OperacaoRequest
    {
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }
    }
}