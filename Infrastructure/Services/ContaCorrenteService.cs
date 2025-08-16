using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Service;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class ContaCorrenteService(IConfiguration config) : IContaCorrenteService
{
    public int GenerateToken(int x)
    {

        return x + 1;
    }
}
