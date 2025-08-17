using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using BancoDigital.Middlewares;
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
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
    ServiceLifetime.Scoped);

//Dependency Injection
//Repository
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
//Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IContaCorrenteService, ContaCorrenteService>();
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7237/api/");
});

// Adiciona Controllers
builder.Services.AddControllers();

// Configuração JWT
var key = Encoding.ASCII.GetBytes("BancoDigital2025CuritibaPRBrasil");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(key),
         ValidateIssuer = false,
         ValidateAudience = false,
         ValidateLifetime = true,
         ClockSkew = TimeSpan.Zero
     };

     // Eventos para customizar resposta
     options.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context =>
         {
             // Token inválido ou assinatura incorreta
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
             context.Response.ContentType = "application/json";
             return context.Response.WriteAsync("{\"error\":\"Token inválido\"}");
         },
         OnChallenge = context =>
         {
             // Token ausente ou expirado
             context.HandleResponse(); // impede comportamento padrão (401)
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
             context.Response.ContentType = "application/json";
             return context.Response.WriteAsync("{\"error\":\"Token expirado ou não fornecido\"}");
         }
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapear Controllers normalmente
app.MapControllers();

app.Run();