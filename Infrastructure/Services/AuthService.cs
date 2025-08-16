using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Models.Auth;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BancoDigital.Middlewares.ErrorHandlingMiddleware;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public AuthService(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public string Login(LoginRequest loginRequest)
        {
            string identificacaoConta = string.Empty;
            var obj = new ContaCorrente();

            if (loginRequest.NumeroConta != 0)
            {
                obj = _contaCorrenteRepository.GetContaByNumeroConta(loginRequest.NumeroConta).Result;
            }
            else if (loginRequest.Cpf != null)
            {
                obj = _contaCorrenteRepository.GetContaByCpf(loginRequest.Cpf).Result;
            }
            else
            {
                throw new ValidationException($"Numero da Conta e Cpf não foram informados");
            }

            //Validações por NUMERO CONTA
            if (loginRequest.Cpf == null || loginRequest.Cpf == "string")
            {
                if (loginRequest.NumeroConta != obj?.Numero && loginRequest.Password == obj.Senha)
                {
                    throw new UnauthorizedException($"USER_UNAUTHORIZED - Numero Conta invalido.");
                }
                if (loginRequest.NumeroConta == obj?.Numero && loginRequest.Password != obj.Senha)
                {
                    throw new UnauthorizedException($"Senha Conta invalida.");
                }
                identificacaoConta = loginRequest.NumeroConta.ToString();
            }
            //Validações por CPF
            if (loginRequest.NumeroConta == 0)
            {
                if (loginRequest.Cpf != obj?.Cpf && loginRequest.Password == obj.Senha)
                {
                    throw new UnauthorizedException($"USER_UNAUTHORIZED - Cpf Invalido.");
                }
                if (loginRequest.Cpf == obj?.Cpf && loginRequest.Password != obj.Senha)
                {
                    throw new UnauthorizedException($"Senha Conta invalida.");
                }
                identificacaoConta = loginRequest.Cpf;
            }
   
           return GeraToken(loginRequest, identificacaoConta);
        }

        private string GeraToken(LoginRequest loginRequest, string identificacaoConta)
        {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("BancoDigital2025CuritibaPRBrasil");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, identificacaoConta) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
        }

    //public async Task<ContaCorrente?> GetContaByNameAsync(string name, CancellationToken ct = default)
    //    => await _repository.GetContaByNameAsync(name, ct);

    //public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
    //    => await _repository.ExistsByUserNameAsync(userName, ct);

    //public async Task<ContaCorrente> AddContaAsync(UsuarioRequest usuario, CancellationToken ct = default)
    //{
    //    //Valida cpf é valido
    //    if (!UtilsGeral.cpfValido(usuario.Cpf))
    //        throw new ValidationException($"INVALID_DOCUMENT Nº - '{usuario.Cpf}' ");

    //    var ultimaContaValida = await _repository.GetUltimoNumeroContaValido(ct);
    //    var obj = new ContaCorrente()
    //    {
    //        IdContaCorrente = Guid.NewGuid(),
    //        Cpf = usuario.Cpf,
    //        Numero = ultimaContaValida != null ? ultimaContaValida.Numero + 1 : 1,
    //        Nome = usuario.Name,
    //        Ativo = 1,
    //        Senha = usuario.Senha,
    //        Saldo = 0
    //    };
    //    return await _repository.AddContaAsync(obj, ct);
    //}

    //public async Task<ContaCorrente> TransferenciaContaAsync(TransferenciaRequest contaTransferenciaRequest, CancellationToken ct = default)
    //{
    //    var contaOrigem = await _repository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaOrigem);
    //    var contaDestino = await _repository.GetContaByNumeroConta(contaTransferenciaRequest.NumeroContaDestino);

    //    //Valida conta Origem existe
    //    if (contaOrigem == null)
    //        throw new NotFoundException($"Conta origem Nº - '{contaTransferenciaRequest.NumeroContaOrigem}' não encontrada.");
    //    //Valida conta Destino existe
    //    if (contaDestino == null)
    //        throw new NotFoundException($"Conta destino Nº - '{contaTransferenciaRequest.NumeroContaDestino}' não encontrada.");

    //    //Valida se possui saldo(valor) para transferencia
    //    if (contaTransferenciaRequest.valor > contaOrigem?.Saldo)
    //    {
    //        throw new ValidationException($"Conta não possui valor para transferência, seu saldo atual é de {contaOrigem.Saldo}");
    //    }

    //    //Retira dinheiro conta origem
    //    contaOrigem.Saldo -= contaTransferenciaRequest.valor;
    //    await _repository.UpdateContaAsync(contaOrigem, ct);

    //    //Adiciona valor na conta destino
    //    contaDestino.Saldo += contaTransferenciaRequest.valor;

    //    return await _repository.UpdateContaAsync(contaDestino, ct);
    //}

}
}