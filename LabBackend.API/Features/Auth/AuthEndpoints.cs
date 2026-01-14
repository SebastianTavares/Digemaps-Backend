using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabBackend.API.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LabBackend.API.Features.Auth;

public record LoginRequest(string Correo, string Contrasena);
public record LoginResponse(string Token, string Nombre, string Rol);

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginAsync)
             .AllowAnonymous();

        return group;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> LoginAsync(
        LoginRequest request,
        LabDbContext db,
        IConfiguration configuration)
    {
        var usuario = await db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UsuCorreo == request.Correo);

        if (usuario is null || usuario.UsuContrasena != request.Contrasena)
        {
            return TypedResults.Unauthorized();
        }

        var token = GenerateJwtToken(usuario.UsuId, usuario.UsuNombre, usuario.UsuRol, configuration);

        return TypedResults.Ok(new LoginResponse(token, usuario.UsuNombre, usuario.UsuRol));
    }

    private static string GenerateJwtToken(int userId, string nombre, string rol, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"]
                     ?? throw new InvalidOperationException("JWT Key is missing in configuration");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Role, rol)
        };

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}