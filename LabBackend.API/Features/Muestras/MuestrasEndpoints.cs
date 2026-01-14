using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Muestras;

// DTOs
public record MuestraDto(
    int Id,
    string CodigoUnico,
    string Nombre,
    string TipoMuestra,
    string Solicitante,
    string Region,
    string Estado,
    string NumOficio,
    string NumLote,
    DateOnly FechaRecepcion,
    DateOnly FechaToma);

public record CreateMuestraRequest(
    string CodigoUnico,
    string Nombre,
    int TipoMuestraId,
    int SolicitanteId,
    int RegionId,
    int EstadoMuestraId,
    string NumOficio,
    string NumLote,
    DateOnly FechaRecepcion,
    string CondicionesRecepcion,
    string MotivoSolicitud,
    string Color,
    string Olor,
    string Sabor,
    string Aspecto,
    string Textura,
    decimal PesoNeto,
    string DpsArea,
    string TomadaPor,
    string EnviadaPor,
    string DireccionToma,
    DateOnly FechaToma);

public static class MuestrasEndpoints
{
    public static RouteGroupBuilder MapMuestrasEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:int}", GetByIdAsync);
        group.MapPost("/", CreateAsync);

        group.RequireAuthorization();

        return group;
    }

    private static async Task<Ok<MuestraDto[]>> GetAllAsync(LabDbContext db)
    {
        var muestras = await db.Muestras
            .AsNoTracking()
            .Include(m => m.TipoMuestra)
            .Include(m => m.Solicitante)
            .Include(m => m.Region)
            .Include(m => m.EstadoMuestra)
            .Select(m => new MuestraDto(
                m.MuestraId,
                m.MuestraCodigoUnico,
                m.MuestraNombre,
                m.TipoMuestra.MuestraTipo,
                m.Solicitante.SolicitanteNombre,
                m.Region.RegionSalud1,
                m.EstadoMuestra.MuestraEstado,
                m.NumOficio,
                m.NumLote,
                m.FechaRecepcion,
                m.FechaToma))
            .ToArrayAsync();

        return TypedResults.Ok(muestras);
    }

    private static async Task<Results<Ok<MuestraDto>, NotFound>> GetByIdAsync(
        int id,
        LabDbContext db)
    {
        var muestra = await db.Muestras
            .AsNoTracking()
            .Include(m => m.TipoMuestra)
            .Include(m => m.Solicitante)
            .Include(m => m.Region)
            .Include(m => m.EstadoMuestra)
            .FirstOrDefaultAsync(m => m.MuestraId == id);

        if (muestra is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new MuestraDto(
            muestra.MuestraId,
            muestra.MuestraCodigoUnico,
            muestra.MuestraNombre,
            muestra.TipoMuestra.MuestraTipo,
            muestra.Solicitante.SolicitanteNombre,
            muestra.Region.RegionSalud1,
            muestra.EstadoMuestra.MuestraEstado,
            muestra.NumOficio,
            muestra.NumLote,
            muestra.FechaRecepcion,
            muestra.FechaToma));
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateAsync(
        CreateMuestraRequest request,
        LabDbContext db)
    {
        var codeExists = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.MuestraCodigoUnico == request.CodigoUnico);

        if (codeExists)
        {
            return TypedResults.BadRequest("El código único ya existe en la base de datos.");
        }

        var muestra = new Muestra
        {
            MuestraCodigoUnico = request.CodigoUnico,
            MuestraNombre = request.Nombre,
            TipoMuestraId = request.TipoMuestraId,
            SolicitanteId = request.SolicitanteId,
            RegionId = request.RegionId,
            EstadoMuestraId = request.EstadoMuestraId,
            NumOficio = request.NumOficio,
            NumLote = request.NumLote,
            FechaRecepcion = request.FechaRecepcion,
            CondicionesRecepcion = request.CondicionesRecepcion,
            MotivoSolicitud = request.MotivoSolicitud,
            Color = request.Color,
            Olor = request.Olor,
            Sabor = request.Sabor,
            Aspecto = request.Aspecto,
            Textura = request.Textura,
            PesoNeto = request.PesoNeto,
            DpsArea = request.DpsArea,
            TomadaPor = request.TomadaPor,
            EnviadaPor = request.EnviadaPor,
            DireccionToma = request.DireccionToma,
            FechaToma = request.FechaToma
        };

        db.Muestras.Add(muestra);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/muestras/{muestra.MuestraId}", muestra.MuestraId);
    }
}