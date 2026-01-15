using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Muestras;

public record CreateDevolucionRequest(string Motivo);

public static class DevolucionesEndpoints
{
    public static RouteGroupBuilder MapDevolucionesEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/muestras/{id:int}/devolver", DevolverAsync);

        group.RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>, Conflict<string>>> DevolverAsync(
        int id,
        CreateDevolucionRequest request,
        LabDbContext db)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var muestra = await db.Muestras
            .FirstOrDefaultAsync(m => m.MuestraId == id);

        if (muestra is null)
        {
            return TypedResults.NotFound();
        }

        var alreadyReturned = await db.DevolucionMuestras
            .AsNoTracking()
            .AnyAsync(d => d.MuestraId == id);

        if (alreadyReturned)
        {
            return TypedResults.Conflict("La muestra ya fue devuelta anteriormente.");
        }

        var devolucion = new DevolucionMuestra
        {
            MuestraId = id,
            MotivoDevolucion = request.Motivo
        };

        db.DevolucionMuestras.Add(devolucion);

        // Prefer "Devuelta", fallback to "Rechazada", then fallback to ID 3, else create dynamically
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

        return TypedResults.Ok();
    }
}