using System;
using System.Collections.Generic;
using LabBackend.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabBackend.API.Data;

public partial class LabDbContext : DbContext
{
    public LabDbContext()
    {
    }

    public LabDbContext(DbContextOptions<LabDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnalisisFisicoquimico> AnalisisFisicoquimicos { get; set; }

    public virtual DbSet<AnalisisMicrobiologico> AnalisisMicrobiologicos { get; set; }

    public virtual DbSet<DevolucionMuestra> DevolucionMuestras { get; set; }

    public virtual DbSet<EstadoMuestra> EstadoMuestras { get; set; }

    public virtual DbSet<Muestra> Muestras { get; set; }

    public virtual DbSet<MuestraUsuarioRol> MuestraUsuarioRols { get; set; }

    public virtual DbSet<RegionSalud> RegionSaluds { get; set; }

    public virtual DbSet<Solicitante> Solicitantes { get; set; }

    public virtual DbSet<TipoMuestra> TipoMuestras { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=Sebastian-Swift\\SEBAS;Database=Digemaps;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalisisFisicoquimico>(entity =>
        {
            entity.HasKey(e => e.AnalisisFqId).HasName("PK__Analisis__A0EE283D85A555E5");

            entity.HasOne(d => d.Muestra).WithOne(p => p.AnalisisFisicoquimico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fisicoquimico_Muestras");
        });

        modelBuilder.Entity<AnalisisMicrobiologico>(entity =>
        {
            entity.HasKey(e => e.AnalisisMicroId).HasName("PK__Analisis__02BE43B19843BF37");

            entity.HasOne(d => d.Muestra).WithOne(p => p.AnalisisMicrobiologico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Microbiologico_Muestras");
        });

        modelBuilder.Entity<DevolucionMuestra>(entity =>
        {
            entity.HasKey(e => e.DevolucionId).HasName("PK__Devoluci__8E0C08B66FB58E25");

            entity.HasOne(d => d.Muestra).WithOne(p => p.DevolucionMuestra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Devolucion_Muestras");
        });

        modelBuilder.Entity<EstadoMuestra>(entity =>
        {
            entity.HasKey(e => e.EstadoMuestraId).HasName("PK__EstadoMu__F3A9A5A0C387BD16");
        });

        modelBuilder.Entity<Muestra>(entity =>
        {
            entity.HasKey(e => e.MuestraId).HasName("PK__Muestras__1DEA251220CBB0DE");

            entity.HasOne(d => d.EstadoMuestra).WithMany(p => p.Muestras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Muestras_Estado");

            entity.HasOne(d => d.Region).WithMany(p => p.Muestras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Muestras_Region");

            entity.HasOne(d => d.Solicitante).WithMany(p => p.Muestras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Muestras_Solicitante");

            entity.HasOne(d => d.TipoMuestra).WithMany(p => p.Muestras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Muestras_Tipo");
        });

        modelBuilder.Entity<MuestraUsuarioRol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MuestraU__3213E83F65CB9472");

            entity.Property(e => e.AsignadoEn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Muestra).WithMany(p => p.MuestraUsuarioRols)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MUR_Muestras");

            entity.HasOne(d => d.Usu).WithMany(p => p.MuestraUsuarioRols)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MUR_Usuario");
        });

        modelBuilder.Entity<RegionSalud>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__RegionSa__01146BAEA7B14325");
        });

        modelBuilder.Entity<Solicitante>(entity =>
        {
            entity.HasKey(e => e.SolicitanteId).HasName("PK__Solicita__F52B1AF7DC5086BA");
        });

        modelBuilder.Entity<TipoMuestra>(entity =>
        {
            entity.HasKey(e => e.TipoMuestraId).HasName("PK__TipoMues__A0B23D97E6FDD1DD");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuId).HasName("PK__Usuario__430A673C9666B1C0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
