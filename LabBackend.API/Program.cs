using System.Text;
using LabBackend.API.Data;
using LabBackend.API.Features.Analisis;
using LabBackend.API.Features.Auth;
using LabBackend.API.Features.Catalogos;
using LabBackend.API.Features.Muestras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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

// 4. OpenAPI / Swagger con Bearer Authentication
builder.Services.AddOpenApi(options =>
{
    // Add Bearer security scheme using document transformer
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Ensure Components exists
        document.Components ??= new OpenApiComponents();
        
        // Ensure SecuritySchemes dictionary exists
        if (document.Components.SecuritySchemes is null)
        {
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>();
        }
        
        // Create and add security scheme
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Ingrese el token JWT"
        };

        // Add global security requirement
        document.Security ??= [];
        var requirement = new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer"), [] }
        };
        document.Security.Add(requirement);

        return Task.CompletedTask;
    });
});

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

    // Interfaz Gráfica Scalar
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

// Muestras
app.MapGroup("/api/muestras").MapMuestrasEndpoints();

// Análisis (Físico y Micro)
app.MapGroup("/api/analisis").MapAnalisisEndpoints();

// Asignaciones
app.MapGroup("/api/asignaciones").MapAsignacionesEndpoints();

// Devoluciones
app.MapGroup("/api/devoluciones").MapDevolucionesEndpoints();

// Catálogos
app.MapGroup("/api/catalogos").MapCatalogosEndpoints();

await app.RunAsync();