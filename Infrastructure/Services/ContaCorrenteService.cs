using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Enum;
using Domain.Models.Request;
using Domain.Utils;
using static BancoDigital.Middlewares.ErrorHandlingMiddleware;

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
                Saldo = 500 //Saldo inicial
            };
            return await _contaCorrenteRepository.AddContaAsync(obj, ct);
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

            //Altera status conta
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
                SaldoAtual = saldoAux.ToString("0.00").Equals("0,00") ? cc.Saldo.ToString("0.00") : saldoAux.ToString("0.00")
            };
            return result;
        }

        public async Task<ContaCorrente> RegistrarDebito(OperacaoRequest request, CancellationToken ct = default)
        {
            var cc = await _contaCorrenteRepository.GetContaByNumeroConta(request.NumeroConta);

            //Valida conta corrente existe
            if (cc == null)
                throw new NotFoundException($"INVALID_ACCOUNT Conta corrente Nº - '{request.NumeroConta}' não encontrada.");

            //Retira dinheiro conta origem
            cc.Saldo -= request.Valor;
            return await _contaCorrenteRepository.UpdateContaAsync(cc, ct);
        }

        public async Task<ContaCorrente> RegistrarCredito(OperacaoRequest request, CancellationToken ct = default)
        {
            var cc = await _contaCorrenteRepository.GetContaByNumeroConta(request.NumeroConta);

            //Valida conta corrente existe
            if (cc == null)
                throw new NotFoundException($"INVALID_ACCOUNT Conta corrente Nº {request.NumeroConta} não encontrada.");

            //Adiciona valor na conta destino
            cc.Saldo += request.Valor;
            return await _contaCorrenteRepository.UpdateContaAsync(cc, ct);
        }
    }
}