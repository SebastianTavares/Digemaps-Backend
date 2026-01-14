using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Domain.Entities;

[Table("Usuario")]
public partial class Usuario
{
    [Key]
    [Column("usu_id")]
    public int UsuId { get; set; }

    [Column("usu_nombre")]
    [StringLength(150)]
    public string UsuNombre { get; set; } = null!;

    [Column("usu_correo")]
    [StringLength(150)]
    public string UsuCorreo { get; set; } = null!;

    [Column("usu_contrasena")]
    [StringLength(255)]
    public string UsuContrasena { get; set; } = null!;

    [Column("usu_rol")]
    [StringLength(50)]
    public string UsuRol { get; set; } = null!;

    [InverseProperty("Usu")]
    public virtual ICollection<MuestraUsuarioRol> MuestraUsuarioRols { get; set; } = new List<MuestraUsuarioRol>();
}
