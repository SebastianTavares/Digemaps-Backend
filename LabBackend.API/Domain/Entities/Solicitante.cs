using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("Solicitante")]
public partial class Solicitante
{
    [Key]
    [Column("solicitante_id")]
    public int SolicitanteId { get; set; }

    [Column("solicitante_nombre")]
    [StringLength(200)]
    public string SolicitanteNombre { get; set; } = null!;

    [Column("solicitante_direccion")]
    [StringLength(255)]
    public string SolicitanteDireccion { get; set; } = null!;

    [Column("solicitante_contacto")]
    [StringLength(100)]
    public string SolicitanteContacto { get; set; } = null!;

    [InverseProperty("Solicitante")]
    public virtual ICollection<Muestra> Muestras { get; set; } = new List<Muestra>();
}
