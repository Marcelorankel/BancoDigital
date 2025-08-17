using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Models.Request;
using static BancoDigital.Middlewares.ErrorHandlingMiddleware;

namespace Infrastructure.Services
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IApiService _apiService;


        public TransferenciaService(IContaCorrenteRepository contaCorrenteRepository,
            ITransferenciaRepository transferenciaRepository,
            IApiService apiService)
        {

            _contaCorrenteRepository = contaCorrenteRepository;
            _transferenciaRepository = transferenciaRepository;
            _apiService = apiService;

        }
        public async Task<Transferencia> TransferenciaContaAsync(TransferenciaRequest request, string token, CancellationToken ct = default)
        {
            var contaOrigem = await _contaCorrenteRepository.GetContaByNumeroConta(request.NumeroContaOrigem);
            var contaDestino = await _contaCorrenteRepository.GetContaByNumeroConta(request.NumeroContaDestino);

            //Valida conta Origem existe
            if (contaOrigem == null)
                throw new NotFoundException($"Conta origem Nº - '{request.NumeroContaOrigem}' não encontrada.");
            //Valida conta Destino existe
            if (contaDestino == null)
                throw new NotFoundException($"Conta destino Nº - '{request.NumeroContaDestino}' não encontrada.");

            //Valida se possui saldo(valor) para transferencia
            if (request.Valor > contaOrigem?.Saldo)
            {
                throw new ValidationException($"Conta não possui valor para transferência, seu saldo atual é de {contaOrigem.Saldo}");
            }

            //Debita valor da conta de origem
            var debitoRequest = new OperacaoRequest
            {
                NumeroConta = request.NumeroContaOrigem,
                Valor = request.Valor
            };

            var resultDebito = await _apiService.PostToController<ContaCorrente>(
                                                                            "contacorrente/RegistrarDebito",
                                                                            debitoRequest,
                                                                            token     
                                                                            );
            
            //Credita valor na conta de destino
            var creditoRequest = new OperacaoRequest
            {
                NumeroConta = request.NumeroContaDestino,
                Valor = request.Valor
            };
            try
            {
               
                var resultCredito = await _apiService.PostToController<ContaCorrente>(
                                                                               "contacorrente/RegistrarCredito",
                                                                               creditoRequest,
                                                                               token
                                                                          );
            }
            catch (Exception)//EFETUA ESTORNO CONTA DEBITADA se ocorrer erro na transferencia
            {
                var resultCredito = await _apiService.PostToController<ContaCorrente>(
                                                                               "contacorrente/RegistrarCredito",
                                                                               debitoRequest,
                                                                               token
                                                                          );
                throw new ValidationException($"Ouve um erro durante a transferência e foi feito o estorno, porfavor tente novamente");
            }
            var transferencia = new Transferencia
            {
                IdTransferencia = Guid.NewGuid(),
                IdContaCorrente_Origem = contaOrigem.IdContaCorrente,
                IdContaCorrente_Destino = contaDestino.IdContaCorrente,
                valor = request.Valor,
                DataMovimento = DateTime.Now
            };
            var res = await _transferenciaRepository.AddTransferenciaAsync(transferencia, ct);

            return res;
        }
    }
}