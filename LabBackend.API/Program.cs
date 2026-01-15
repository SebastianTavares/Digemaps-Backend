using System.Text;
using LabBackend.API.Data;
using LabBackend.API.Features.Analisis;
using LabBackend.API.Features.Auth;
using LabBackend.API.Features.Catalogos;
using LabBackend.API.Features.Muestras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

// 1. CAMBIO CLAVE: Usamos CreateBuilder normal (No SlimBuilder)
// Esto habilita todas las funciones estándar de .NET y evita errores de AOT.
var builder = WebApplication.CreateBuilder(args);

// 2. Base de Datos (SQL Server)
// Eliminamos ".UseModel(...)" para que EF Core funcione dinámicamente sin errores.
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

// 4. OpenAPI / Swagger
builder.Services.AddOpenApi();

// 5. CORS (Permitir todo para desarrollo rápido)
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
    // Genera el JSON de OpenAPI
    app.MapOpenApi();

    // Interfaz Gráfica Scalar (Funciona perfecto en modo estándar también)
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// Mapeo de Endpoints (Features)
// ==========================================

// Auth
app.MapGroup("/api/auth").MapAuthEndpoints();

// Muestras (Incluye Asignaciones y Devoluciones si están en la misma carpeta/namespace)
// Si Asignaciones/Devoluciones están en clases separadas, asegúrate de tener los 'using' arriba.
app.MapGroup("/api/muestras").MapMuestrasEndpoints();

// Análisis (Físico y Micro)
app.MapGroup("/api/analisis").MapAnalisisEndpoints();

// Asignaciones (Si creaste una clase separada AsignacionesEndpoints)
app.MapGroup("/api/asignaciones").MapAsignacionesEndpoints();

// Devoluciones (Si creaste una clase separada DevolucionesEndpoints)
app.MapGroup("/api/devoluciones").MapDevolucionesEndpoints();

// Catálogos
app.MapGroup("/api/catalogos").MapCatalogosEndpoints();

app.Run();

// NOTA: He eliminado toda la clase 'AppJsonSerializerContext' del final.
// En modo estándar (.NET JIT), NO LA NECESITAS. .NET serializa todo automáticamente.