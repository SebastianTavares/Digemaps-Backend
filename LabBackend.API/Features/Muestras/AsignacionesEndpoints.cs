using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Muestras;

// DTOs
public record CreateAsignacionRequest(
    int MuestraId,
    int UsuarioId
);

public record AsignacionDto(
    int Id,
    int MuestraId,
    string MuestraCodigo,
    string MuestraNombre,
    int UsuarioId,
    string UsuarioNombre,
    DateTime AsignadoEn);

public record AssignUserRequest(int UsuarioId);

public record UpdateAsignacionRequest(int UsuarioId);

public static class AsignacionesEndpoints
{
    public static RouteGroupBuilder MapAsignacionesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllAsync);
        group.MapPost("/", CreateAsync);
        group.MapGet("/{id:int}", GetByIdAsync);
        group.MapPost("/muestras/{muestraId:int}/asignar", AssignUserAsync);
        group.MapPut("/{id:int}", UpdateAsync);
        group.MapDelete("/{id:int}", DeleteAsync);

        group.RequireAuthorization();

        return group;
    }
    private static async Task<
    Results<Created<AsignacionDto>, BadRequest<string>, Conflict<string>>>
CreateAsync(
    CreateAsignacionRequest request,
    LabDbContext db)
    {
        var muestra = await db.Muestras
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MuestraId == request.MuestraId);

        if (muestra is null)
        {
            return TypedResults.BadRequest("La muestra no existe.");
        }

        var usuario = await db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UsuId == request.UsuarioId);

        if (usuario is null)
        {
            return TypedResults.BadRequest("El usuario no existe.");
        }

        var exists = await db.MuestraUsuarioRols
            .AnyAsync(x => x.MuestraId == request.MuestraId
                        && x.UsuId == request.UsuarioId);

        //if (exists)
        //{
        //    return TypedResults.Conflict("La asignación ya existe.");
        //}

        var asignacion = new MuestraUsuarioRol
        {
            MuestraId = request.MuestraId,
            UsuId = request.UsuarioId,
            AsignadoEn = DateTime.UtcNow
        };

        db.MuestraUsuarioRols.Add(asignacion);
        await db.SaveChangesAsync();

        var dto = new AsignacionDto(
            asignacion.Id,
            muestra.MuestraId,
            muestra.MuestraCodigoUnico,
            muestra.MuestraNombre,
            usuario.UsuId,
            usuario.UsuNombre,
            asignacion.AsignadoEn
        );

        return TypedResults.Created($"/api/asignaciones/{asignacion.Id}", dto);
    }


    private static async Task<Ok<AsignacionDto[]>> GetAllAsync(LabDbContext db)
    {
        var asignaciones = await db.MuestraUsuarioRols
            .AsNoTracking()
            .Include(a => a.Muestra)
            .Include(a => a.Usu)
            .Select(a => new AsignacionDto(
                a.Id,
                a.MuestraId,
                a.Muestra.MuestraCodigoUnico,
                a.Muestra.MuestraNombre,
                a.UsuId,
                a.Usu.UsuNombre,
                a.AsignadoEn))
            .ToArrayAsync();

        return TypedResults.Ok(asignaciones);
    }

    private static async Task<Results<Ok<AsignacionDto>, NotFound>> GetByIdAsync(
        int id,
        LabDbContext db)
    {
        var asignacion = await db.MuestraUsuarioRols
            .AsNoTracking()
            .Include(a => a.Muestra)
            .Include(a => a.Usu)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asignacion is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new AsignacionDto(
            asignacion.Id,
            asignacion.MuestraId,
            asignacion.Muestra.MuestraCodigoUnico,
            asignacion.Muestra.MuestraNombre,
            asignacion.UsuId,
            asignacion.Usu.UsuNombre,
            asignacion.AsignadoEn));
    }

    private static async Task<Results<Created<int>, NotFound<string>, BadRequest<string>, Conflict<string>>> AssignUserAsync(
        int muestraId,
        AssignUserRequest request,
        LabDbContext db)
    {
        var muestraExists = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.MuestraId == muestraId);

        if (!muestraExists)
        {
            return TypedResults.NotFound("Muestra no existe.");
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
            .AnyAsync(x => x.MuestraId == muestraId && x.UsuId == request.UsuarioId);

        if (assignmentExists)
        {
            return TypedResults.Conflict("La asignación ya existe.");
        }

        var asignacion = new MuestraUsuarioRol
        {
            MuestraId = muestraId,
            UsuId = request.UsuarioId,
            AsignadoEn = DateTime.Now
        };

        db.MuestraUsuarioRols.Add(asignacion);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/asignaciones/{asignacion.Id}", asignacion.Id);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<string>, Conflict<string>>> UpdateAsync(
        int id,
        UpdateAsignacionRequest request,
        LabDbContext db)
    {
        var asignacion = await db.MuestraUsuarioRols.FindAsync(id);

        if (asignacion is null)
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

        // Check if new assignment would create a duplicate
        var duplicateExists = await db.MuestraUsuarioRols
            .AsNoTracking()
            .AnyAsync(x => x.MuestraId == asignacion.MuestraId 
                        && x.UsuId == request.UsuarioId 
                        && x.Id != id);

        if (duplicateExists)
        {
            return TypedResults.Conflict("Ya existe una asignación para este usuario y muestra.");
        }

        asignacion.UsuId = request.UsuarioId;
        asignacion.AsignadoEn = DateTime.Now;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        int id,
        LabDbContext db)
    {
        var asignacion = await db.MuestraUsuarioRols.FindAsync(id);

        if (asignacion is null)
        {
            return TypedResults.NotFound();
        }

        db.MuestraUsuarioRols.Remove(asignacion);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}