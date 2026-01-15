using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Muestras;

// DTOs
public record DevolucionDto(
    int Id,
    int MuestraId,
    string MuestraCodigo,
    string MuestraNombre,
    string MotivoDevolucion);

public record CreateDevolucionRequest(string Motivo);

public record UpdateDevolucionRequest(string Motivo);

public static class DevolucionesEndpoints
{
    public static RouteGroupBuilder MapDevolucionesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:int}", GetByIdAsync);
        group.MapPost("/muestras/{muestraId:int}/devolver", DevolverAsync);
        group.MapPut("/{id:int}", UpdateAsync);
        group.MapDelete("/{id:int}", DeleteAsync);

        group.RequireAuthorization();

        return group;
    }

    private static async Task<Ok<DevolucionDto[]>> GetAllAsync(LabDbContext db)
    {
        var devoluciones = await db.DevolucionMuestras
            .AsNoTracking()
            .Include(d => d.Muestra)
            .Select(d => new DevolucionDto(
                d.DevolucionId,
                d.MuestraId,
                d.Muestra.MuestraCodigoUnico,
                d.Muestra.MuestraNombre,
                d.MotivoDevolucion))
            .ToArrayAsync();

        return TypedResults.Ok(devoluciones);
    }

    private static async Task<Results<Ok<DevolucionDto>, NotFound>> GetByIdAsync(
        int id,
        LabDbContext db)
    {
        var devolucion = await db.DevolucionMuestras
            .AsNoTracking()
            .Include(d => d.Muestra)
            .FirstOrDefaultAsync(d => d.DevolucionId == id);

        if (devolucion is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new DevolucionDto(
            devolucion.DevolucionId,
            devolucion.MuestraId,
            devolucion.Muestra.MuestraCodigoUnico,
            devolucion.Muestra.MuestraNombre,
            devolucion.MotivoDevolucion));
    }

    private static async Task<Results<Created<int>, NotFound, Conflict<string>>> DevolverAsync(
        int muestraId,
        CreateDevolucionRequest request,
        LabDbContext db)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var muestra = await db.Muestras
            .FirstOrDefaultAsync(m => m.MuestraId == muestraId);

        if (muestra is null)
        {
            return TypedResults.NotFound();
        }

        var alreadyReturned = await db.DevolucionMuestras
            .AsNoTracking()
            .AnyAsync(d => d.MuestraId == muestraId);

        //if (alreadyReturned)
        //{
        //    return TypedResults.Conflict("La muestra ya fue devuelta anteriormente.");
        //}

        var devolucion = new DevolucionMuestra
        {
            MuestraId = muestraId,
            MotivoDevolucion = request.Motivo
        };

        db.DevolucionMuestras.Add(devolucion);

        // Update muestra status to "Devuelta"
        var estado = await db.EstadoMuestras
            .FirstOrDefaultAsync(e => e.MuestraEstado == "Devuelta")
            ?? await db.EstadoMuestras.FirstOrDefaultAsync(e => e.MuestraEstado == "Rechazada");

        if (estado is null)
        {
            estado = await db.EstadoMuestras.FirstOrDefaultAsync(e => e.EstadoMuestraId == 3);

            if (estado is null)
            {
                estado = new EstadoMuestra
                {
                    MuestraEstado = "Devuelta"
                };

                db.EstadoMuestras.Add(estado);
                await db.SaveChangesAsync();
            }
        }

        muestra.EstadoMuestraId = estado.EstadoMuestraId;

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return TypedResults.Created($"/api/devoluciones/{devolucion.DevolucionId}", devolucion.DevolucionId);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateAsync(
        int id,
        UpdateDevolucionRequest request,
        LabDbContext db)
    {
        var devolucion = await db.DevolucionMuestras.FindAsync(id);

        if (devolucion is null)
        {
            return TypedResults.NotFound();
        }

        devolucion.MotivoDevolucion = request.Motivo;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAsync(
        int id,
        LabDbContext db)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var devolucion = await db.DevolucionMuestras
            .Include(d => d.Muestra)
            .FirstOrDefaultAsync(d => d.DevolucionId == id);

        if (devolucion is null)
        {
            return TypedResults.NotFound();
        }

        // Revert muestra status to "Pendiente" or first available status
        var estadoPendiente = await db.EstadoMuestras
            .FirstOrDefaultAsync(e => e.MuestraEstado == "Pendiente")
            ?? await db.EstadoMuestras.FirstOrDefaultAsync(e => e.EstadoMuestraId == 1);

        if (estadoPendiente is not null)
        {
            devolucion.Muestra.EstadoMuestraId = estadoPendiente.EstadoMuestraId;
        }

        db.DevolucionMuestras.Remove(devolucion);
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return TypedResults.NoContent();
    }
}