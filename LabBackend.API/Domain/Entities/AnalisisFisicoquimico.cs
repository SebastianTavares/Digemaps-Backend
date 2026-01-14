using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("AnalisisFisicoquimico")]
[Index("MuestraId", Name = "UQ__Analisis__1DEA251306E38FAF", IsUnique = true)]
public partial class AnalisisFisicoquimico
{
    [Key]
    [Column("analisis_fq_id")]
    public int AnalisisFqId { get; set; }

    [Column("muestra_id")]
    public int MuestraId { get; set; }

    [Column("fecha_entrega")]
    public DateOnly FechaEntrega { get; set; }

    [Column("fecha_vencimiento")]
    public DateOnly FechaVencimiento { get; set; }

    [Column("acidez", TypeName = "decimal(18, 4)")]
    public decimal Acidez { get; set; }

    [Column("cloro_residual", TypeName = "decimal(18, 4)")]
    public decimal CloroResidual { get; set; }

    [Column("cenizas", TypeName = "decimal(18, 4)")]
    public decimal Cenizas { get; set; }

    [Column("cumarina", TypeName = "decimal(18, 4)")]
    public decimal Cumarina { get; set; }

    [Column("colorante")]
    [StringLength(100)]
    public string Colorante { get; set; } = null!;

    [Column("densidad", TypeName = "decimal(18, 4)")]
    public decimal Densidad { get; set; }

    [Column("dureza", TypeName = "decimal(18, 4)")]
    public decimal Dureza { get; set; }

    [Column("extracto_seco", TypeName = "decimal(18, 4)")]
    public decimal ExtractoSeco { get; set; }

    [Column("fecula", TypeName = "decimal(18, 4)")]
    public decimal Fecula { get; set; }

    [Column("grado_alcoholico", TypeName = "decimal(18, 4)")]
    public decimal GradoAlcoholico { get; set; }

    [Column("humedad", TypeName = "decimal(18, 4)")]
    public decimal Humedad { get; set; }

    [Column("indice_refraccion", TypeName = "decimal(18, 4)")]
    public decimal IndiceRefraccion { get; set; }

    [Column("indice_acidez", TypeName = "decimal(18, 4)")]
    public decimal IndiceAcidez { get; set; }

    [Column("indice_rancidez", TypeName = "decimal(18, 4)")]
    public decimal IndiceRancidez { get; set; }

    [Column("materia_grasa_cualitativa")]
    [StringLength(100)]
    public string MateriaGrasaCualitativa { get; set; } = null!;

    [Column("materia_grasa_cuantitativa", TypeName = "decimal(18, 4)")]
    public decimal MateriaGrasaCuantitativa { get; set; }

    [Column("ph", TypeName = "decimal(18, 4)")]
    public decimal Ph { get; set; }

    [Column("prueba_ebar")]
    [StringLength(100)]
    public string PruebaEbar { get; set; } = null!;

    [Column("solidos_totales", TypeName = "decimal(18, 4)")]
    public decimal SolidosTotales { get; set; }

    [Column("tiempo_coccion")]
    [StringLength(100)]
    public string TiempoCoccion { get; set; } = null!;

    [Column("otras_determinaciones")]
    public string OtrasDeterminaciones { get; set; } = null!;

    [Column("referencia")]
    [StringLength(255)]
    public string Referencia { get; set; } = null!;

    [Column("observaciones")]
    public string Observaciones { get; set; } = null!;

    [Column("apto_consumo_humano")]
    [StringLength(50)]
    public string AptoConsumoHumano { get; set; } = null!;

    [ForeignKey("MuestraId")]
    [InverseProperty("AnalisisFisicoquimico")]
    public virtual Muestra Muestra { get; set; } = null!;
}
