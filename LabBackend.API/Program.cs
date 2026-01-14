using System.Text;
using System.Text.Json.Serialization;
using LabBackend.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);

// 1. Configuración JSON para AOT
// NOTA: Aquí iremos registrando los DTOs para que funcionen en modo nativo.
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
            ValidateIssuer = false,   // Simplificado para desarrollo rápido
            ValidateAudience = false, // Simplificado para desarrollo rápido
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
    // Nota: Para ver la UI visual, accede a /openapi/v1.json y úsalo en un visor, 
    // o agrega Scalar/SwaggerUI si tienes tiempo extra.
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de Endpoints (Aquí irán tus features)
// app.MapGroup("/api/auth").MapAuthEndpoints(); 

app.Run();

//Contexto de Serialización para AOT
// IMPORTANTE: Cada vez que crees un DTO nuevo que se envíe o reciba por API, 
// debes agregarlo aquí con [JsonSerializable(typeof(TuClase))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(DateTime))]
// [JsonSerializable(typeof(LoginRequest))] // Descomentar cuando crees el Login
// [JsonSerializable(typeof(LoginResponse))] 
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}