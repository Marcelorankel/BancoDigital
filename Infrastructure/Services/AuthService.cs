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
    }
}