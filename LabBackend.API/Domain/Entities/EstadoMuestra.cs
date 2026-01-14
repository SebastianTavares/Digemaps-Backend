using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("EstadoMuestra")]
[Index("MuestraEstado", Name = "UQ__EstadoMu__ED79EA640A0373FD", IsUnique = true)]
public partial class EstadoMuestra
{
    [Key]
    [Column("estado_muestra_id")]
    public int EstadoMuestraId { get; set; }

    [Column("muestra_estado")]
    [StringLength(50)]
    public string MuestraEstado { get; set; } = null!;

    [InverseProperty("EstadoMuestra")]
    public virtual ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
}
