using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("DevolucionMuestra")]
[Index("MuestraId", Name = "UQ__Devoluci__1DEA2513CD200D1A", IsUnique = true)]
public partial class DevolucionMuestra
{
    [Key]
    [Column("devolucion_id")]
    public int DevolucionId { get; set; }

    [Column("muestra_id")]
    public int MuestraId { get; set; }

    [Column("motivo_devolucion")]
    public string MotivoDevolucion { get; set; } = null!;

    [ForeignKey("MuestraId")]
    [InverseProperty("DevolucionMuestra")]
    public virtual Muestra Muestra { get; set; } = null!;
}
