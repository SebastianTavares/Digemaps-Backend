using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("AnalisisMicrobiologico")]
[Index("MuestraId", Name = "UQ__Analisis__1DEA25131E15622C", IsUnique = true)]
public partial class AnalisisMicrobiologico
{
    [Key]
    [Column("analisis_micro_id")]
    public int AnalisisMicroId { get; set; }

    [Column("muestra_id")]
    public int MuestraId { get; set; }

    [Column("res_microorganismos_aerobios")]
    [StringLength(100)]
    public string ResMicroorganismosAerobios { get; set; } = null!;

    [Column("res_recuento_coliformes")]
    [StringLength(100)]
    public string ResRecuentoColiformes { get; set; } = null!;

    [Column("res_coliformes_totales")]
    [StringLength(100)]
    public string ResColiformesTotales { get; set; } = null!;

    [Column("res_pseudomonas_spp")]
    [StringLength(100)]
    public string ResPseudomonasSpp { get; set; } = null!;

    [Column("res_e_coli")]
    [StringLength(100)]
    public string ResEColi { get; set; } = null!;

    [Column("res_salmonella_spp")]
    [StringLength(100)]
    public string ResSalmonellaSpp { get; set; } = null!;

    [Column("res_estafilococos_aureus")]
    [StringLength(100)]
    public string ResEstafilococosAureus { get; set; } = null!;

    [Column("res_hongos")]
    [StringLength(100)]
    public string ResHongos { get; set; } = null!;

    [Column("res_levaduras")]
    [StringLength(100)]
    public string ResLevaduras { get; set; } = null!;

    [Column("res_esterilidad_comercial")]
    [StringLength(100)]
    public string ResEsterilidadComercial { get; set; } = null!;

    [Column("res_listeria_monocytogenes")]
    [StringLength(100)]
    public string ResListeriaMonocytogenes { get; set; } = null!;

    [Column("metodologia_referencia")]
    [StringLength(255)]
    public string MetodologiaReferencia { get; set; } = null!;

    [Column("equipos")]
    [StringLength(255)]
    public string Equipos { get; set; } = null!;

    [Column("observaciones")]
    public string Observaciones { get; set; } = null!;

    [Column("apto_para_consumo")]
    [StringLength(50)]
    public string AptoParaConsumo { get; set; } = null!;

    [Column("es_copia")]
    public bool EsCopia { get; set; }

    [ForeignKey("MuestraId")]
    [InverseProperty("AnalisisMicrobiologico")]
    public virtual Muestra Muestra { get; set; } = null!;
}
