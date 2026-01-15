using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Muestras;

public record AssignUserRequest(int UsuarioId);

public static class AsignacionesEndpoints
{
    public static RouteGroupBuilder MapAsignacionesEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/muestras/{id:int}/asignar", AssignUserAsync);

        group.RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>, Conflict<string>>> AssignUserAsync(
        int id,
        AssignUserRequest request,
        LabDbContext db)
    {
        var muestraExists = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.MuestraId == id);

        if (!muestraExists)
        {
            return TypedResults.NotFound();
        }

        var usuarioExists = await db.Usuarios
            .AsNoTracking()
            .AnyAsync(u => u.UsuId == request.UsuarioId);

        if (!usuarioExists)
        {
            return TypedResults.BadRequest("Usuario no existe.");
        }

        var assignmentExists = await db.MuestraUsuarioRols
            .AsNoTracking()
            .AnyAsync(x => x.MuestraId == id && x.UsuId == request.UsuarioId);

        if (assignmentExists)
        {
            return TypedResults.Conflict("La asignación ya existe.");
        }

        var asignacion = new MuestraUsuarioRol
        {
            MuestraId = id,
            UsuId = request.UsuarioId,
            AsignadoEn = DateTime.Now
        };

        db.MuestraUsuarioRols.Add(asignacion);
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }
}