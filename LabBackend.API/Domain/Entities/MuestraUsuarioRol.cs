using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("MuestraUsuarioRol")]
[Index("MuestraId", "UsuId", Name = "UQ_Muestra_Usuario", IsUnique = true)]
public partial class MuestraUsuarioRol
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("muestra_id")]
    public int MuestraId { get; set; }

    [Column("usu_id")]
    public int UsuId { get; set; }

    [Column("asignado_en", TypeName = "datetime")]
    public DateTime AsignadoEn { get; set; }

    [ForeignKey("MuestraId")]
    [InverseProperty("MuestraUsuarioRols")]
    public virtual Muestra Muestra { get; set; } = null!;

    [ForeignKey("UsuId")]
    [InverseProperty("MuestraUsuarioRols")]
    public virtual Usuario Usu { get; set; } = null!;
}
