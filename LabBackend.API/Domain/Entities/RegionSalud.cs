using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("RegionSalud")]
[Index("RegionSalud1", Name = "UQ__RegionSa__9A04A1B4BEEF5E6F", IsUnique = true)]
public partial class RegionSalud
{
    [Key]
    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("region_salud")]
    [StringLength(100)]
    public string RegionSalud1 { get; set; } = null!;

    [InverseProperty("Region")]
    public virtual ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
}
