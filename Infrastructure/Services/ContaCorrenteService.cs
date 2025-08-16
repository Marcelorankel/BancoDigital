using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Utils;
using Domain.Models.Request;
using System.Xml.Linq;
using static BancoDigital.Middlewares.ErrorHandlingMiddleware;

namespace Infrastructure.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        private readonly IContaCorrenteRepository _repository;

        public ContaCorrenteService(IContaCorrenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ContaCorrente?> GetContaByNameAsync(string name, CancellationToken ct = default)
            => await _repository.GetContaByNameAsync(name, ct);

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
            => await _repository.ExistsByUserNameAsync(userName, ct);

        public async Task<ContaCorrente> AddContaAsync(UsuarioRequest usuario, CancellationToken ct = default)
        {
            //Valida cpf � valido
            if (!UtilsGeral.cpfValido(usuario.Cpf))
                throw new ValidationException($"INVALID_DOCUMENT N� - '{usuario.Cpf}' ");

            var ultimaContaValida = await _repository.GetUltimoNumeroContaValido(ct);
            var obj = new ContaCorrente()
            {
                IdContaCorrente = Guid.NewGuid(),
                Cpf = usuario.Cpf,
                Numero = ultimaContaValida != null ? ultimaContaValida.Numero + 1 : 1,
                Nome = usuario.Name,
                Ativo = 1,
                Senha = usuario.Senha,
                Saldo = 0
            };
            return await _repository.AddContaAsync(obj, ct);
        }

        public async Task<ContaCorrente> TransferenciaContaAsync(TransferenciaRequest contaTransferenciaRequest, CancellationToken ct = default)
        {
            var contaOrigem = await _repository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaOrigem);
            var contaDestino = await _repository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaDestino);

            //Valida conta Origem existe
            if (contaOrigem == null)
                throw new NotFoundException($"Conta origem N� - '{contaTransferenciaRequest.NumeroContaOrigem}' n�o encontrada.");
            //Valida conta Destino existe
            if (contaDestino == null)
                throw new NotFoundException($"Conta destino N� - '{contaTransferenciaRequest.NumeroContaDestino}' n�o encontrada.");

            //Valida se possui saldo(valor) para transferencia
            if (contaTransferenciaRequest.valor > contaOrigem?.Saldo)
            {
                throw new ValidationException($"Conta n�o possui valor para transfer�ncia, seu saldo atual � de {contaOrigem.Saldo}");
            }

            //Retira dinheiro conta origem
            contaOrigem.Saldo -= contaTransferenciaRequest.valor;
            await _repository.UpdateContaAsync(contaOrigem, ct);

            //Adiciona valor na conta destino
            contaDestino.Saldo += contaTransferenciaRequest.valor;

            return await _repository.UpdateContaAsync(contaDestino, ct);
        }

    }
}