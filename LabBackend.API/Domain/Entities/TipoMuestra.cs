using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("TipoMuestra")]
[Index("MuestraTipo", Name = "UQ__TipoMues__DB889C2717B4127F", IsUnique = true)]
public partial class TipoMuestra
{
    [Key]
    [Column("tipo_muestra_id")]
    public int TipoMuestraId { get; set; }

    [Column("muestra_tipo")]
    [StringLength(100)]
    public string MuestraTipo { get; set; } = null!;

    [InverseProperty("TipoMuestra")]
    public virtual ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
}
