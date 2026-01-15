using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using LabBackend.API.Features.Catalogos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabBackend.API.Features.Auth;

public record LoginRequest(string Correo, string Contrasena);
public record LoginResponse(string Token, string Nombre, string Rol);
public record RegisterRequest(string Nombre, string Correo, string Contrasena, string Rol);

public record UsuarioResponse(
    int Id,
    string Nombre,
    string Correo,
    string Rol
);


public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", LoginAsync)
             .AllowAnonymous();

        group.MapPost("/register", RegisterAsync)
             .AllowAnonymous();

        group.MapGet("/users", GetAllUsersAsync)
            .RequireAuthorization();

        return group;
    }

    private static async Task<Ok<List<UsuarioResponse>>> GetAllUsersAsync(
    LabDbContext db)
    {
        var usuarios = await db.Usuarios
            .AsNoTracking()
            .Select(u => new UsuarioResponse(
                u.UsuId,
                u.UsuNombre,
                u.UsuCorreo,
                u.UsuRol
            ))
            .ToListAsync();

        return TypedResults.Ok(usuarios);
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

    private static async Task<Results<Created<int>, Conflict<string>>> RegisterAsync(
        RegisterRequest request,
        LabDbContext db)
    {
        var emailExists = await db.Usuarios
            .AsNoTracking()
            .AnyAsync(u => u.UsuCorreo == request.Correo);

        if (emailExists)
        {
            return TypedResults.Conflict("El correo electrónico ya está registrado.");
        }

        // TODO: Hash the password before storing. Use BCrypt or Argon2 for production.
        var usuario = new Usuario
        {
            UsuNombre = request.Nombre,
            UsuCorreo = request.Correo,
            UsuContrasena = request.Contrasena,
            UsuRol = request.Rol
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/auth/users/{usuario.UsuId}", usuario.UsuId);
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