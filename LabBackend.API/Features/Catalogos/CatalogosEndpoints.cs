using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Catalogos;

// DTOs for TipoMuestra
public record TipoMuestraDto(int Id, string Tipo);
public record CreateTipoMuestraRequest(string Tipo);
public record UpdateTipoMuestraRequest(string Tipo);

// DTOs for RegionSalud
public record RegionSaludDto(int Id, string Region);
public record CreateRegionSaludRequest(string Region);
public record UpdateRegionSaludRequest(string Region);

// DTOs for Solicitante
public record SolicitanteDto(int Id, string Nombre, string Direccion, string Contacto);
public record CreateSolicitanteRequest(string Nombre, string Direccion, string Contacto);
public record UpdateSolicitanteRequest(string Nombre, string Direccion, string Contacto);

// DTOs for EstadoMuestra
public record EstadoMuestraDto(int Id, string Estado);
public record CreateEstadoMuestraRequest(string Estado);
public record UpdateEstadoMuestraRequest(string Estado);

public static class CatalogosEndpoints
{
    public static RouteGroupBuilder MapCatalogosEndpoints(this RouteGroupBuilder group)
    {
        // TipoMuestra endpoints
        group.MapGet("/tipos", GetTiposAsync);
        group.MapGet("/tipos/{id:int}", GetTipoByIdAsync);
        group.MapPost("/tipos", CreateTipoAsync);
        group.MapPut("/tipos/{id:int}", UpdateTipoAsync);
        group.MapDelete("/tipos/{id:int}", DeleteTipoAsync);

        // EstadoMuestra endpoints
        group.MapGet("/estados", GetEstadosAsync);
        group.MapGet("/estados/{id:int}", GetEstadoByIdAsync);
        group.MapPost("/estados", CreateEstadoAsync);
        group.MapPut("/estados/{id:int}", UpdateEstadoAsync);
        group.MapDelete("/estados/{id:int}", DeleteEstadoAsync);

        // RegionSalud endpoints
        group.MapGet("/regiones", GetRegionesAsync);
        group.MapGet("/regiones/{id:int}", GetRegionByIdAsync);
        group.MapPost("/regiones", CreateRegionAsync);
        group.MapPut("/regiones/{id:int}", UpdateRegionAsync);
        group.MapDelete("/regiones/{id:int}", DeleteRegionAsync);

        // Solicitante endpoints
        group.MapGet("/solicitantes", GetSolicitantesAsync);
        group.MapGet("/solicitantes/{id:int}", GetSolicitanteByIdAsync);
        group.MapPost("/solicitantes", CreateSolicitanteAsync);
        group.MapPut("/solicitantes/{id:int}", UpdateSolicitanteAsync);
        group.MapDelete("/solicitantes/{id:int}", DeleteSolicitanteAsync);

        group.RequireAuthorization();

        return group;
    }

    // ========== TIPO MUESTRA ==========

    private static async Task<Ok<TipoMuestraDto[]>> GetTiposAsync(LabDbContext db)
    {
        var tipos = await db.TipoMuestras
            .AsNoTracking()
            .Select(t => new TipoMuestraDto(t.TipoMuestraId, t.MuestraTipo))
            .ToArrayAsync();

        return TypedResults.Ok(tipos);
    }

    private static async Task<Results<Ok<TipoMuestraDto>, NotFound>> GetTipoByIdAsync(
        int id,
        LabDbContext db)
    {
        var tipo = await db.TipoMuestras
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TipoMuestraId == id);

        if (tipo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new TipoMuestraDto(tipo.TipoMuestraId, tipo.MuestraTipo));
    }

    private static async Task<Results<Created<int>, Conflict<string>>> CreateTipoAsync(
        CreateTipoMuestraRequest request,
        LabDbContext db)
    {
        var exists = await db.TipoMuestras
            .AsNoTracking()
            .AnyAsync(t => t.MuestraTipo == request.Tipo);

        if (exists)
        {
            return TypedResults.Conflict("El tipo de muestra ya existe.");
        }

        var tipo = new TipoMuestra { MuestraTipo = request.Tipo };

        db.TipoMuestras.Add(tipo);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/catalogos/tipos/{tipo.TipoMuestraId}", tipo.TipoMuestraId);
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> UpdateTipoAsync(
        int id,
        UpdateTipoMuestraRequest request,
        LabDbContext db)
    {
        var tipo = await db.TipoMuestras.FindAsync(id);

        if (tipo is null)
        {
            return TypedResults.NotFound();
        }

        var duplicateExists = await db.TipoMuestras
            .AsNoTracking()
            .AnyAsync(t => t.MuestraTipo == request.Tipo && t.TipoMuestraId != id);

        if (duplicateExists)
        {
            return TypedResults.Conflict("El tipo de muestra ya existe.");
        }

        tipo.MuestraTipo = request.Tipo;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> DeleteTipoAsync(
        int id,
        LabDbContext db)
    {
        var tipo = await db.TipoMuestras.FindAsync(id);

        if (tipo is null)
        {
            return TypedResults.NotFound();
        }

        var hasRelatedMuestras = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.TipoMuestraId == id);

        if (hasRelatedMuestras)
        {
            return TypedResults.Conflict("No se puede eliminar el tipo porque tiene muestras asociadas.");
        }

        db.TipoMuestras.Remove(tipo);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // ========== ESTADO MUESTRA ==========

    private static async Task<Ok<EstadoMuestraDto[]>> GetEstadosAsync(LabDbContext db)
    {
        var estados = await db.EstadoMuestras
            .AsNoTracking()
            .Select(e => new EstadoMuestraDto(e.EstadoMuestraId, e.MuestraEstado))
            .ToArrayAsync();

        return TypedResults.Ok(estados);
    }

    private static async Task<Results<Ok<EstadoMuestraDto>, NotFound>> GetEstadoByIdAsync(
        int id,
        LabDbContext db)
    {
        var estado = await db.EstadoMuestras
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EstadoMuestraId == id);

        if (estado is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new EstadoMuestraDto(estado.EstadoMuestraId, estado.MuestraEstado));
    }

    private static async Task<Results<Created<int>, Conflict<string>>> CreateEstadoAsync(
        CreateEstadoMuestraRequest request,
        LabDbContext db)
    {
        var exists = await db.EstadoMuestras
            .AsNoTracking()
            .AnyAsync(e => e.MuestraEstado == request.Estado);

        if (exists)
        {
            return TypedResults.Conflict("El estado de muestra ya existe.");
        }

        var estado = new EstadoMuestra { MuestraEstado = request.Estado };

        db.EstadoMuestras.Add(estado);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/catalogos/estados/{estado.EstadoMuestraId}", estado.EstadoMuestraId);
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> UpdateEstadoAsync(
        int id,
        UpdateEstadoMuestraRequest request,
        LabDbContext db)
    {
        var estado = await db.EstadoMuestras.FindAsync(id);

        if (estado is null)
        {
            return TypedResults.NotFound();
        }

        var duplicateExists = await db.EstadoMuestras
            .AsNoTracking()
            .AnyAsync(e => e.MuestraEstado == request.Estado && e.EstadoMuestraId != id);

        if (duplicateExists)
        {
            return TypedResults.Conflict("El estado de muestra ya existe.");
        }

        estado.MuestraEstado = request.Estado;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> DeleteEstadoAsync(
        int id,
        LabDbContext db)
    {
        var estado = await db.EstadoMuestras.FindAsync(id);

        if (estado is null)
        {
            return TypedResults.NotFound();
        }

        var hasRelatedMuestras = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.EstadoMuestraId == id);

        if (hasRelatedMuestras)
        {
            return TypedResults.Conflict("No se puede eliminar el estado porque tiene muestras asociadas.");
        }

        db.EstadoMuestras.Remove(estado);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // ========== REGION SALUD ==========

    private static async Task<Ok<RegionSaludDto[]>> GetRegionesAsync(LabDbContext db)
    {
        var regiones = await db.RegionSaluds
            .AsNoTracking()
            .Select(r => new RegionSaludDto(r.RegionId, r.RegionSalud1))
            .ToArrayAsync();

        return TypedResults.Ok(regiones);
    }

    private static async Task<Results<Ok<RegionSaludDto>, NotFound>> GetRegionByIdAsync(
        int id,
        LabDbContext db)
    {
        var region = await db.RegionSaluds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RegionId == id);

        if (region is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new RegionSaludDto(region.RegionId, region.RegionSalud1));
    }

    private static async Task<Results<Created<int>, Conflict<string>>> CreateRegionAsync(
        CreateRegionSaludRequest request,
        LabDbContext db)
    {
        var exists = await db.RegionSaluds
            .AsNoTracking()
            .AnyAsync(r => r.RegionSalud1 == request.Region);

        if (exists)
        {
            return TypedResults.Conflict("La región de salud ya existe.");
        }

        var region = new RegionSalud { RegionSalud1 = request.Region };

        db.RegionSaluds.Add(region);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/catalogos/regiones/{region.RegionId}", region.RegionId);
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> UpdateRegionAsync(
        int id,
        UpdateRegionSaludRequest request,
        LabDbContext db)
    {
        var region = await db.RegionSaluds.FindAsync(id);

        if (region is null)
        {
            return TypedResults.NotFound();
        }

        var duplicateExists = await db.RegionSaluds
            .AsNoTracking()
            .AnyAsync(r => r.RegionSalud1 == request.Region && r.RegionId != id);

        if (duplicateExists)
        {
            return TypedResults.Conflict("La región de salud ya existe.");
        }

        region.RegionSalud1 = request.Region;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> DeleteRegionAsync(
        int id,
        LabDbContext db)
    {
        var region = await db.RegionSaluds.FindAsync(id);

        if (region is null)
        {
            return TypedResults.NotFound();
        }

        var hasRelatedMuestras = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.RegionId == id);

        if (hasRelatedMuestras)
        {
            return TypedResults.Conflict("No se puede eliminar la región porque tiene muestras asociadas.");
        }

        db.RegionSaluds.Remove(region);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // ========== SOLICITANTE ==========

    private static async Task<Ok<SolicitanteDto[]>> GetSolicitantesAsync(LabDbContext db)
    {
        var solicitantes = await db.Solicitantes
            .AsNoTracking()
            .Select(s => new SolicitanteDto(
                s.SolicitanteId,
                s.SolicitanteNombre,
                s.SolicitanteDireccion,
                s.SolicitanteContacto))
            .ToArrayAsync();

        return TypedResults.Ok(solicitantes);
    }

    private static async Task<Results<Ok<SolicitanteDto>, NotFound>> GetSolicitanteByIdAsync(
        int id,
        LabDbContext db)
    {
        var solicitante = await db.Solicitantes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SolicitanteId == id);

        if (solicitante is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new SolicitanteDto(
            solicitante.SolicitanteId,
            solicitante.SolicitanteNombre,
            solicitante.SolicitanteDireccion,
            solicitante.SolicitanteContacto));
    }

    private static async Task<Created<int>> CreateSolicitanteAsync(
        CreateSolicitanteRequest request,
        LabDbContext db)
    {
        var solicitante = new Solicitante
        {
            SolicitanteNombre = request.Nombre,
            SolicitanteDireccion = request.Direccion,
            SolicitanteContacto = request.Contacto
        };

        db.Solicitantes.Add(solicitante);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/catalogos/solicitantes/{solicitante.SolicitanteId}", solicitante.SolicitanteId);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateSolicitanteAsync(
        int id,
        UpdateSolicitanteRequest request,
        LabDbContext db)
    {
        var solicitante = await db.Solicitantes.FindAsync(id);

        if (solicitante is null)
        {
            return TypedResults.NotFound();
        }

        solicitante.SolicitanteNombre = request.Nombre;
        solicitante.SolicitanteDireccion = request.Direccion;
        solicitante.SolicitanteContacto = request.Contacto;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, Conflict<string>>> DeleteSolicitanteAsync(
        int id,
        LabDbContext db)
    {
        var solicitante = await db.Solicitantes.FindAsync(id);

        if (solicitante is null)
        {
            return TypedResults.NotFound();
        }

        var hasRelatedMuestras = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.SolicitanteId == id);

        if (hasRelatedMuestras)
        {
            return TypedResults.Conflict("No se puede eliminar el solicitante porque tiene muestras asociadas.");
        }

        db.Solicitantes.Remove(solicitante);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}