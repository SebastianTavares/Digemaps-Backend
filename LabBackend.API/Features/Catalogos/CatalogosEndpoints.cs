using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Catalogos;

public static class CatalogosEndpoints
{
    public static RouteGroupBuilder MapCatalogosEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/tipos", GetTiposAsync);
        group.MapGet("/estados", GetEstadosAsync);
        group.MapGet("/regiones", GetRegionesAsync);
        group.MapGet("/solicitantes", GetSolicitantesAsync);

        group.RequireAuthorization();

        return group;
    }

    private static async Task<Ok<List<TipoMuestra>>> GetTiposAsync(LabDbContext db) =>
        TypedResults.Ok(await db.TipoMuestras
            .AsNoTracking()
            .ToListAsync());

    private static async Task<Ok<List<EstadoMuestra>>> GetEstadosAsync(LabDbContext db) =>
        TypedResults.Ok(await db.EstadoMuestras
            .AsNoTracking()
            .ToListAsync());

    private static async Task<Ok<List<RegionSalud>>> GetRegionesAsync(LabDbContext db) =>
        TypedResults.Ok(await db.RegionSaluds
            .AsNoTracking()
            .ToListAsync());

    private static async Task<Ok<List<Solicitante>>> GetSolicitantesAsync(LabDbContext db) =>
        TypedResults.Ok(await db.Solicitantes
            .AsNoTracking()
            .ToListAsync());
}