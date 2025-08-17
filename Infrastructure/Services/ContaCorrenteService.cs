using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Utils;
using Domain.Models.Request;
using System.Xml.Linq;
using static BancoDigital.Middlewares.ErrorHandlingMiddleware;
using Domain.Enum;

namespace Infrastructure.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ContaCorrenteService(IContaCorrenteRepository contaCorrenteRepository, IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ContaCorrente?> GetContaByNameAsync(string name, CancellationToken ct = default)
            => await _contaCorrenteRepository.GetContaByNameAsync(name, ct);

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
            => await _contaCorrenteRepository.ExistsByUserNameAsync(userName, ct);

        public async Task<ContaCorrente> AddContaAsync(UsuarioRequest usuario, CancellationToken ct = default)
        {
            //Valida cpf é valido
            if (!UtilsGeral.cpfValido(usuario.Cpf))
                throw new ValidationException($"INVALID_DOCUMENT Nº - '{usuario.Cpf}' ");

            var ultimaContaValida = await _contaCorrenteRepository.GetUltimoNumeroContaValido(ct);
            var obj = new ContaCorrente()
            {
                IdContaCorrente = Guid.NewGuid(),
                Cpf = usuario.Cpf,
                Numero = ultimaContaValida != null ? ultimaContaValida.Numero + 1 : 1,
                Nome = usuario.Name,
                Ativo = 1,
                Senha = usuario.Senha,
                Salt = "salt",
                Saldo = 0
            };
            return await _contaCorrenteRepository.AddContaAsync(obj, ct);
        }

        public async Task<ContaCorrente> TransferenciaContaAsync(TransferenciaRequest contaTransferenciaRequest, CancellationToken ct = default)
        {
            var contaOrigem = await _contaCorrenteRepository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaOrigem);
            var contaDestino = await _contaCorrenteRepository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaDestino);

            //Valida conta Origem existe
            if (contaOrigem == null)
                throw new NotFoundException($"Conta origem Nº - '{contaTransferenciaRequest.NumeroContaOrigem}' não encontrada.");
            //Valida conta Destino existe
            if (contaDestino == null)
                throw new NotFoundException($"Conta destino Nº - '{contaTransferenciaRequest.NumeroContaDestino}' não encontrada.");

            //Valida se possui saldo(valor) para transferencia
            if (contaTransferenciaRequest.Valor > contaOrigem?.Saldo)
            {
                throw new ValidationException($"Conta não possui valor para transferência, seu saldo atual é de {contaOrigem.Saldo}");
            }

            //Retira dinheiro conta origem
            contaOrigem.Saldo -= contaTransferenciaRequest.Valor;
            await _contaCorrenteRepository.UpdateContaAsync(contaOrigem, ct);

            //Adiciona valor na conta destino
            contaDestino.Saldo += contaTransferenciaRequest.Valor;

            return await _contaCorrenteRepository.UpdateContaAsync(contaDestino, ct);
        }

        public async Task<ContaCorrente> AlterarStatusContaAsync(StatusContaCorrenteRequest request, string numeroContaToken, CancellationToken ct = default)
        {
            //Busca conta
            var cc = await _contaCorrenteRepository.GetContaByNumeroConta(request.NumeroConta);

            //Valida conta Origem existe
            if (cc == null)
            {
                throw new NotFoundException($"INVALID_ACCOUNT Conta Corrente Nº - '{request.NumeroConta}' não encontrada.");
            }

            //Validação senha CONTA
            if (request.NumeroConta == cc?.Numero && request.Senha != cc.Senha)
            {
                throw new UnauthorizedException($"Senha Conta invalida.");
            }

            //Desativa conta
            cc.Ativo = (int)request.eStatusConta;

            return await _contaCorrenteRepository.UpdateContaAsync(cc, ct);
        }

        public async Task<Movimento> MovimentoContaAsync(MovimentacaoContaCorrenteRequest request, string numeroContaToken, CancellationToken ct = default)
        {
            try
            {
                int codTipoTransferencia = (int)UtilsTransacao.FromCodigo(request.TipoTransferencia);
                //Verifica se Numero Conta foi informado, senão usa o numero da conta do TOKEN
                int numeroConta = 0;
                if (request.NumeroConta != 0)
                {
                    numeroConta = request.NumeroConta;
                }
                else
                {
                    numeroConta = Convert.ToInt32(numeroContaToken);
                }

                var cc = await _contaCorrenteRepository.GetContaByNumeroConta(numeroConta);

                //Valida conta corrente existe
                if (cc == null)
                    throw new NotFoundException($"INVALID_ACCOUNT Conta corrente Nº - '{numeroConta}' não encontrada.");
                //Valida se conta 1 - ativa
                if (cc.Ativo == 0)
                    throw new ValidationException($"INACTIVE_ACCOUNT Conta corrente Nº - '{numeroConta}' está inativa.");

                //Valida D - Debito ou C - Credito
                if (codTipoTransferencia != (int)eTipoTransferencia.Debito && codTipoTransferencia != (int)eTipoTransferencia.Credito)
                {
                    throw new ValidationException($"INVALID_TYPE apenas tipos de tranferencias debito e credito podem ser aceitos.");
                }

                //Valida tipo “crédito” só pode ser aceito, caso o número da conta seja diferente do usuário logado; TIPO: INVALID_TYPE.
                if (numeroContaToken != null)
                {
                    if (request.NumeroConta != Convert.ToInt32(numeroContaToken) && codTipoTransferencia == (int)eTipoTransferencia.Debito)
                    {
                        throw new ValidationException($"INVALID_TYPE apenas o tipo “crédito” pode ser aceito caso o número da conta seja diferente do usuário logado");
                    }
                }

                //Salva Movimento
                var movimento = new Movimento
                {
                    Idmovimento = Guid.NewGuid(),
                    IdContaCorrente = cc.IdContaCorrente,
                    TipoMovimento = request.TipoTransferencia,
                    DataMovimento = DateTime.Now,
                    Valor = request.Valor
                };
                return await _movimentoRepository.AddMovimentoAsync(movimento, ct);
            }
            catch (Exception ex)
            {

                throw new ValidationException($"Ocorreu um erro {ex.Message}");
            }
        }

        public async Task<ContaCorrenteResult> ObterSaldoContaAsync(int numeroConta, string numeroContaToken, CancellationToken ct = default)
        {
            var cc = await _contaCorrenteRepository.GetContaByNumeroConta(numeroConta);

            //Valida conta corrente existe
            if (cc == null)
                throw new NotFoundException($"INVALID_ACCOUNT Apenas contas correntes cadastradas podem consultar o saldo.");
            //Valida se conta 1 - ativa
            if (cc.Ativo == 0)
                throw new ValidationException($"INACTIVE_ACCOUNT Apenas contas correntes ativas podem consultar o saldo.");

            var lstMovimentacoes = await _movimentoRepository.ObterMovimentosPorIdContaCorrenteAsync(cc.IdContaCorrente);

            decimal saldoAux = 0;
            foreach (var item in lstMovimentacoes)
            {
                if (item.TipoMovimento == UtilsTransacao.ToCodigo(eTipoTransferencia.Debito))
                {
                    saldoAux -= item.Valor;

                }else if (item.TipoMovimento == UtilsTransacao.ToCodigo(eTipoTransferencia.Credito))
                {
                    saldoAux += item.Valor;
                }
            }
            //Salva Movimento
            var result = new ContaCorrenteResult
            {
                NumeroConta = cc.Numero,
                NomeTitular = cc.Nome,
                DtHraConsulta = DateTime.Now,
                SaldoAtual = saldoAux.ToString("0.00")
            };
            return result;
        }

    }
}