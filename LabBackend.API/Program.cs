using System.Text;
using System.Text.Json.Serialization;
using LabBackend.API.Data;
using LabBackend.API.Features.Analisis;
using LabBackend.API.Features.Auth;
using LabBackend.API.Features.Muestras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);

// 1. Configuración JSON para AOT
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// 2. Base de Datos (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<LabDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 4. OpenAPI (Nativo)
builder.Services.AddOpenApi();

// 5. CORS (Para permitir peticiones desde el Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Pipeline de Peticiones
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de Endpoints
app.MapGroup("/api/auth").MapAuthEndpoints();
app.MapGroup("/api/muestras").MapMuestrasEndpoints();
app.MapGroup("/api/analisis").MapAnalisisEndpoints();

app.Run();

// Contexto de Serialización para AOT
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(DateTime))]
// Auth DTOs
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
// Muestras DTOs
[JsonSerializable(typeof(MuestraDto))]
[JsonSerializable(typeof(MuestraDto[]))]
[JsonSerializable(typeof(CreateMuestraRequest))]
// Análisis Fisicoquímico DTOs
[JsonSerializable(typeof(AnalisisFisicoquimicoDto))]
[JsonSerializable(typeof(CreateAnalisisFisicoquimicoRequest))]
// Análisis Microbiológico DTOs
[JsonSerializable(typeof(AnalisisMicrobiologicoDto))]
[JsonSerializable(typeof(CreateAnalisisMicrobiologicoRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}