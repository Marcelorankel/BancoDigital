using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Recupera a string de conexão do MySQL
var connectionString = builder.Configuration.GetConnectionString("MySql");

// Registra o DbContext com MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dependency Injection
//Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
//Service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IContaCorrenteService, ContaCorrenteService>();

// Adiciona Controllers
builder.Services.AddControllers();

// Configuração JWT
var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui_super_segura");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Authorization
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API com JWT", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Insira o token JWT sem o prefixo 'Bearer '",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Ativa Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API com JWT v1");
        c.RoutePrefix = string.Empty; // Para acessar Swagger na raiz, opcional
    });
}

// Middleware que libera Swagger e OpenAPI sem autenticação
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.StartsWith("/swagger") || path.StartsWith("/openapi"))
    {
        await next();
        return;
    }
    await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapear Controllers normalmente
app.MapControllers();

app.Run();