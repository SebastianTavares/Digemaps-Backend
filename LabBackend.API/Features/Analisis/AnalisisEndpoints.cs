using LabBackend.API.Data;
using LabBackend.API.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Features.Analisis;

// DTOs for Fisicoquímico
public record AnalisisFisicoquimicoDto(
    int Id,
    int MuestraId,
    string MuestraCodigo,
    DateOnly FechaEntrega,
    DateOnly FechaVencimiento,
    decimal Acidez,
    decimal CloroResidual,
    decimal Ph,
    decimal Humedad,
    decimal Cenizas,
    decimal Densidad,
    string AptoConsumoHumano);

public record CreateAnalisisFisicoquimicoRequest(
    DateOnly FechaEntrega,
    DateOnly FechaVencimiento,
    decimal Acidez,
    decimal CloroResidual,
    decimal Cenizas,
    decimal Cumarina,
    string Colorante,
    decimal Densidad,
    decimal Dureza,
    decimal ExtractoSeco,
    decimal Fecula,
    decimal GradoAlcoholico,
    decimal Humedad,
    decimal IndiceRefraccion,
    decimal IndiceAcidez,
    decimal IndiceRancidez,
    string MateriaGrasaCualitativa,
    decimal MateriaGrasaCuantitativa,
    decimal Ph,
    string PruebaEbar,
    decimal SolidosTotales,
    string TiempoCoccion,
    string OtrasDeterminaciones,
    string Referencia,
    string Observaciones,
    string AptoConsumoHumano);

public record UpdateAnalisisFisicoquimicoRequest(
    DateOnly FechaEntrega,
    DateOnly FechaVencimiento,
    decimal Acidez,
    decimal CloroResidual,
    decimal Cenizas,
    decimal Cumarina,
    string Colorante,
    decimal Densidad,
    decimal Dureza,
    decimal ExtractoSeco,
    decimal Fecula,
    decimal GradoAlcoholico,
    decimal Humedad,
    decimal IndiceRefraccion,
    decimal IndiceAcidez,
    decimal IndiceRancidez,
    string MateriaGrasaCualitativa,
    decimal MateriaGrasaCuantitativa,
    decimal Ph,
    string PruebaEbar,
    decimal SolidosTotales,
    string TiempoCoccion,
    string OtrasDeterminaciones,
    string Referencia,
    string Observaciones,
    string AptoConsumoHumano);

// DTOs for Microbiológico
public record AnalisisMicrobiologicoDto(
    int Id,
    int MuestraId,
    string MuestraCodigo,
    string ResMicroorganismosAerobios,
    string ResEColi,
    string ResSalmonellaSpp,
    string AptoParaConsumo,
    bool EsCopia);

public record CreateAnalisisMicrobiologicoRequest(
    string ResMicroorganismosAerobios,
    string ResRecuentoColiformes,
    string ResColiformesTotales,
    string ResPseudomonasSpp,
    string ResEColi,
    string ResSalmonellaSpp,
    string ResEstafilococosAureus,
    string ResHongos,
    string ResLevaduras,
    string ResEsterilidadComercial,
    string ResListeriaMonocytogenes,
    string MetodologiaReferencia,
    string Equipos,
    string Observaciones,
    string AptoParaConsumo,
    bool EsCopia);

public record UpdateAnalisisMicrobiologicoRequest(
    string ResMicroorganismosAerobios,
    string ResRecuentoColiformes,
    string ResColiformesTotales,
    string ResPseudomonasSpp,
    string ResEColi,
    string ResSalmonellaSpp,
    string ResEstafilococosAureus,
    string ResHongos,
    string ResLevaduras,
    string ResEsterilidadComercial,
    string ResListeriaMonocytogenes,
    string MetodologiaReferencia,
    string Equipos,
    string Observaciones,
    string AptoParaConsumo,
    bool EsCopia);

public static class AnalisisEndpoints
{
    public static RouteGroupBuilder MapAnalisisEndpoints(this RouteGroupBuilder group)
    {
        // Fisicoquímico endpoints
        group.MapGet("/{muestraId:int}/fisicoquimico", GetFisicoquimicoByMuestraAsync);
        group.MapPost("/{muestraId:int}/fisicoquimico", CreateFisicoquimicoAsync);
        group.MapPut("/fisicoquimico/{id:int}", UpdateFisicoquimicoAsync);
        group.MapDelete("/fisicoquimico/{id:int}", DeleteFisicoquimicoAsync);

        // Microbiológico endpoints
        group.MapGet("/{muestraId:int}/microbiologico", GetMicrobiologicoByMuestraAsync);
        group.MapPost("/{muestraId:int}/microbiologico", CreateMicrobiologicoAsync);
        group.MapPut("/microbiologico/{id:int}", UpdateMicrobiologicoAsync);
        group.MapDelete("/microbiologico/{id:int}", DeleteMicrobiologicoAsync);

        group.RequireAuthorization();

        return group;
    }

    // ========== FISICOQUÍMICO ==========

    private static async Task<Results<Ok<AnalisisFisicoquimicoDto>, NotFound>> GetFisicoquimicoByMuestraAsync(
        int muestraId,
        LabDbContext db)
    {
        var analisis = await db.AnalisisFisicoquimicos
            .AsNoTracking()
            .Include(a => a.Muestra)
            .FirstOrDefaultAsync(a => a.MuestraId == muestraId);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new AnalisisFisicoquimicoDto(
            analisis.AnalisisFqId,
            analisis.MuestraId,
            analisis.Muestra.MuestraCodigoUnico,
            analisis.FechaEntrega,
            analisis.FechaVencimiento,
            analisis.Acidez,
            analisis.CloroResidual,
            analisis.Ph,
            analisis.Humedad,
            analisis.Cenizas,
            analisis.Densidad,
            analisis.AptoConsumoHumano));
    }

    private static async Task<Results<Created<int>, NotFound<string>, BadRequest<string>>> CreateFisicoquimicoAsync(
        int muestraId,
        CreateAnalisisFisicoquimicoRequest request,
        LabDbContext db)
    {
        // Verify muestra exists
        var muestraExists = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.MuestraId == muestraId);

        if (!muestraExists)
        {
            return TypedResults.NotFound($"Muestra con ID {muestraId} no encontrada.");
        }

        // Verify 1:1 relation - analysis doesn't already exist
        var analysisExists = await db.AnalisisFisicoquimicos
            .AsNoTracking()
            .AnyAsync(a => a.MuestraId == muestraId);

        if (analysisExists)
        {
            return TypedResults.BadRequest($"Ya existe un análisis fisicoquímico para la muestra {muestraId}.");
        }

        var analisis = new AnalisisFisicoquimico
        {
            MuestraId = muestraId,
            FechaEntrega = request.FechaEntrega,
            FechaVencimiento = request.FechaVencimiento,
            Acidez = request.Acidez,
            CloroResidual = request.CloroResidual,
            Cenizas = request.Cenizas,
            Cumarina = request.Cumarina,
            Colorante = request.Colorante,
            Densidad = request.Densidad,
            Dureza = request.Dureza,
            ExtractoSeco = request.ExtractoSeco,
            Fecula = request.Fecula,
            GradoAlcoholico = request.GradoAlcoholico,
            Humedad = request.Humedad,
            IndiceRefraccion = request.IndiceRefraccion,
            IndiceAcidez = request.IndiceAcidez,
            IndiceRancidez = request.IndiceRancidez,
            MateriaGrasaCualitativa = request.MateriaGrasaCualitativa,
            MateriaGrasaCuantitativa = request.MateriaGrasaCuantitativa,
            Ph = request.Ph,
            PruebaEbar = request.PruebaEbar,
            SolidosTotales = request.SolidosTotales,
            TiempoCoccion = request.TiempoCoccion,
            OtrasDeterminaciones = request.OtrasDeterminaciones,
            Referencia = request.Referencia,
            Observaciones = request.Observaciones,
            AptoConsumoHumano = request.AptoConsumoHumano
        };

        db.AnalisisFisicoquimicos.Add(analisis);
        await db.SaveChangesAsync();

        return TypedResults.Created(
            $"/api/muestras/{muestraId}/fisicoquimico",
            analisis.AnalisisFqId);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateFisicoquimicoAsync(
        int id,
        UpdateAnalisisFisicoquimicoRequest request,
        LabDbContext db)
    {
        var analisis = await db.AnalisisFisicoquimicos.FindAsync(id);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        analisis.FechaEntrega = request.FechaEntrega;
        analisis.FechaVencimiento = request.FechaVencimiento;
        analisis.Acidez = request.Acidez;
        analisis.CloroResidual = request.CloroResidual;
        analisis.Cenizas = request.Cenizas;
        analisis.Cumarina = request.Cumarina;
        analisis.Colorante = request.Colorante;
        analisis.Densidad = request.Densidad;
        analisis.Dureza = request.Dureza;
        analisis.ExtractoSeco = request.ExtractoSeco;
        analisis.Fecula = request.Fecula;
        analisis.GradoAlcoholico = request.GradoAlcoholico;
        analisis.Humedad = request.Humedad;
        analisis.IndiceRefraccion = request.IndiceRefraccion;
        analisis.IndiceAcidez = request.IndiceAcidez;
        analisis.IndiceRancidez = request.IndiceRancidez;
        analisis.MateriaGrasaCualitativa = request.MateriaGrasaCualitativa;
        analisis.MateriaGrasaCuantitativa = request.MateriaGrasaCuantitativa;
        analisis.Ph = request.Ph;
        analisis.PruebaEbar = request.PruebaEbar;
        analisis.SolidosTotales = request.SolidosTotales;
        analisis.TiempoCoccion = request.TiempoCoccion;
        analisis.OtrasDeterminaciones = request.OtrasDeterminaciones;
        analisis.Referencia = request.Referencia;
        analisis.Observaciones = request.Observaciones;
        analisis.AptoConsumoHumano = request.AptoConsumoHumano;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteFisicoquimicoAsync(
        int id,
        LabDbContext db)
    {
        var analisis = await db.AnalisisFisicoquimicos.FindAsync(id);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        db.AnalisisFisicoquimicos.Remove(analisis);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // ========== MICROBIOLÓGICO ==========

    private static async Task<Results<Ok<AnalisisMicrobiologicoDto>, NotFound>> GetMicrobiologicoByMuestraAsync(
        int muestraId,
        LabDbContext db)
    {
        var analisis = await db.AnalisisMicrobiologicos
            .AsNoTracking()
            .Include(a => a.Muestra)
            .FirstOrDefaultAsync(a => a.MuestraId == muestraId);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new AnalisisMicrobiologicoDto(
            analisis.AnalisisMicroId,
            analisis.MuestraId,
            analisis.Muestra.MuestraCodigoUnico,
            analisis.ResMicroorganismosAerobios,
            analisis.ResEColi,
            analisis.ResSalmonellaSpp,
            analisis.AptoParaConsumo,
            analisis.EsCopia));
    }

    private static async Task<Results<Created<int>, NotFound<string>, BadRequest<string>>> CreateMicrobiologicoAsync(
        int muestraId,
        CreateAnalisisMicrobiologicoRequest request,
        LabDbContext db)
    {
        // Verify muestra exists
        var muestraExists = await db.Muestras
            .AsNoTracking()
            .AnyAsync(m => m.MuestraId == muestraId);

        if (!muestraExists)
        {
            return TypedResults.NotFound($"Muestra con ID {muestraId} no encontrada.");
        }

        // Verify 1:1 relation - analysis doesn't already exist
        var analysisExists = await db.AnalisisMicrobiologicos
            .AsNoTracking()
            .AnyAsync(a => a.MuestraId == muestraId);

        if (analysisExists)
        {
            return TypedResults.BadRequest($"Ya existe un análisis microbiológico para la muestra {muestraId}.");
        }

        var analisis = new AnalisisMicrobiologico
        {
            MuestraId = muestraId,
            ResMicroorganismosAerobios = request.ResMicroorganismosAerobios,
            ResRecuentoColiformes = request.ResRecuentoColiformes,
            ResColiformesTotales = request.ResColiformesTotales,
            ResPseudomonasSpp = request.ResPseudomonasSpp,
            ResEColi = request.ResEColi,
            ResSalmonellaSpp = request.ResSalmonellaSpp,
            ResEstafilococosAureus = request.ResEstafilococosAureus,
            ResHongos = request.ResHongos,
            ResLevaduras = request.ResLevaduras,
            ResEsterilidadComercial = request.ResEsterilidadComercial,
            ResListeriaMonocytogenes = request.ResListeriaMonocytogenes,
            MetodologiaReferencia = request.MetodologiaReferencia,
            Equipos = request.Equipos,
            Observaciones = request.Observaciones,
            AptoParaConsumo = request.AptoParaConsumo,
            EsCopia = request.EsCopia
        };

        db.AnalisisMicrobiologicos.Add(analisis);
        await db.SaveChangesAsync();

        return TypedResults.Created(
            $"/api/muestras/{muestraId}/microbiologico",
            analisis.AnalisisMicroId);
    }

    private static async Task<Results<NoContent, NotFound>> UpdateMicrobiologicoAsync(
        int id,
        UpdateAnalisisMicrobiologicoRequest request,
        LabDbContext db)
    {
        var analisis = await db.AnalisisMicrobiologicos.FindAsync(id);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        analisis.ResMicroorganismosAerobios = request.ResMicroorganismosAerobios;
        analisis.ResRecuentoColiformes = request.ResRecuentoColiformes;
        analisis.ResColiformesTotales = request.ResColiformesTotales;
        analisis.ResPseudomonasSpp = request.ResPseudomonasSpp;
        analisis.ResEColi = request.ResEColi;
        analisis.ResSalmonellaSpp = request.ResSalmonellaSpp;
        analisis.ResEstafilococosAureus = request.ResEstafilococosAureus;
        analisis.ResHongos = request.ResHongos;
        analisis.ResLevaduras = request.ResLevaduras;
        analisis.ResEsterilidadComercial = request.ResEsterilidadComercial;
        analisis.ResListeriaMonocytogenes = request.ResListeriaMonocytogenes;
        analisis.MetodologiaReferencia = request.MetodologiaReferencia;
        analisis.Equipos = request.Equipos;
        analisis.Observaciones = request.Observaciones;
        analisis.AptoParaConsumo = request.AptoParaConsumo;
        analisis.EsCopia = request.EsCopia;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteMicrobiologicoAsync(
        int id,
        LabDbContext db)
    {
        var analisis = await db.AnalisisMicrobiologicos.FindAsync(id);

        if (analisis is null)
        {
            return TypedResults.NotFound();
        }

        db.AnalisisMicrobiologicos.Remove(analisis);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}