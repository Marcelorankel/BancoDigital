using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Domain.Entities;
using Domain.Models.Request;
using System.Xml.Linq;

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
            var ultimaContaValida = await _repository.GetUltimoNumeroContaValido(ct);
            var obj = new ContaCorrente()
            {
                IdContaCorrente = Guid.NewGuid(),
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
                throw new InvalidOperationException($"Conta origem Nº - '{contaTransferenciaRequest.NumeroContaOrigem}' não encontrada.");
            //Valida conta Destino existe
            if (contaDestino == null)
                throw new InvalidOperationException($"Conta destino Nº - '{contaTransferenciaRequest.NumeroContaDestino}' não encontrada.");

            //Valida se possui saldo(valor) para transferencia
            if (contaTransferenciaRequest.valor > contaOrigem?.Saldo)
            {
                throw new InvalidOperationException($"Conta não possui valor para transferência, seu saldo atual é de {contaOrigem.Saldo}");
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