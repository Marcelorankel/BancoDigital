using Domain.Enum;

namespace Domain.Utils
{
    public static class UtilsTransacao
    {
        public static string ToCodigo(this eTipoTransferencia tipo)
        {
            return tipo switch
            {
                eTipoTransferencia.Credito => "C",
                eTipoTransferencia.Debito => "D",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static eTipoTransferencia FromCodigo(string codigo)
        {
            return codigo switch
            {
                "C" => eTipoTransferencia.Credito,
                "D" => eTipoTransferencia.Debito,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}