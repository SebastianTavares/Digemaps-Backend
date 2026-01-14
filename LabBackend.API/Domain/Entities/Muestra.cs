using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Index("MuestraCodigoUnico", Name = "UQ__Muestras__D86BDFABCFD871C4", IsUnique = true)]
public partial class Muestra
{
    [Key]
    [Column("muestra_id")]
    public int MuestraId { get; set; }

    [Column("muestra_codigo_unico")]
    [StringLength(50)]
    public string MuestraCodigoUnico { get; set; } = null!;

    [Column("muestra_nombre")]
    [StringLength(200)]
    public string MuestraNombre { get; set; } = null!;

    [Column("tipo_muestra_id")]
    public int TipoMuestraId { get; set; }

    [Column("solicitante_id")]
    public int SolicitanteId { get; set; }

    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("estado_muestra_id")]
    public int EstadoMuestraId { get; set; }

    [Column("num_oficio")]
    [StringLength(100)]
    public string NumOficio { get; set; } = null!;

    [Column("num_lote")]
    [StringLength(100)]
    public string NumLote { get; set; } = null!;

    [Column("fecha_recepcion")]
    public DateOnly FechaRecepcion { get; set; }

    [Column("condiciones_recepcion")]
    public string CondicionesRecepcion { get; set; } = null!;

    [Column("motivo_solicitud")]
    public string MotivoSolicitud { get; set; } = null!;

    [Column("color")]
    [StringLength(50)]
    public string Color { get; set; } = null!;

    [Column("olor")]
    [StringLength(50)]
    public string Olor { get; set; } = null!;

    [Column("sabor")]
    [StringLength(50)]
    public string Sabor { get; set; } = null!;

    [Column("aspecto")]
    [StringLength(100)]
    public string Aspecto { get; set; } = null!;

    [Column("textura")]
    [StringLength(100)]
    public string Textura { get; set; } = null!;

    [Column("peso_neto", TypeName = "decimal(18, 4)")]
    public decimal PesoNeto { get; set; }

    [Column("dps_area")]
    [StringLength(150)]
    public string DpsArea { get; set; } = null!;

    [Column("tomada_por")]
    [StringLength(150)]
    public string TomadaPor { get; set; } = null!;

    [Column("enviada_por")]
    [StringLength(150)]
    public string EnviadaPor { get; set; } = null!;

    [Column("direccion_toma")]
    [StringLength(255)]
    public string DireccionToma { get; set; } = null!;

    [Column("fecha_toma")]
    public DateOnly FechaToma { get; set; }

    [InverseProperty("Muestra")]
    public virtual AnalisisFisicoquimico? AnalisisFisicoquimico { get; set; }

    [InverseProperty("Muestra")]
    public virtual AnalisisMicrobiologico? AnalisisMicrobiologico { get; set; }

    [InverseProperty("Muestra")]
    public virtual DevolucionMuestra? DevolucionMuestra { get; set; }

    [ForeignKey("EstadoMuestraId")]
    [InverseProperty("Muestras")]
    public virtual EstadoMuestra EstadoMuestra { get; set; } = null!;

    [InverseProperty("Muestra")]
    public virtual ICollection<MuestraUsuarioRol> MuestraUsuarioRols { get; set; } = new List<MuestraUsuarioRol>();

    [ForeignKey("RegionId")]
    [InverseProperty("Muestras")]
    public virtual RegionSalud Region { get; set; } = null!;

    [ForeignKey("SolicitanteId")]
    [InverseProperty("Muestras")]
    public virtual Solicitante Solicitante { get; set; } = null!;

    [ForeignKey("TipoMuestraId")]
    [InverseProperty("Muestras")]
    public virtual TipoMuestra TipoMuestra { get; set; } = null!;
}
