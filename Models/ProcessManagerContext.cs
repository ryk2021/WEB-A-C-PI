using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ApiProcess.Models
{
    public partial class ProcessManagerContext : DbContext
    {
        public ProcessManagerContext()
        {
        }

        public ProcessManagerContext(DbContextOptions<ProcessManagerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CatAreas> CatAreas { get; set; }
        public virtual DbSet<CatConfiguracionesProcess> CatConfiguracionesProcess { get; set; }
        public virtual DbSet<CatPais> CatPais { get; set; }
        public virtual DbSet<CatPerfiles> CatPerfiles { get; set; }
        public virtual DbSet<CatPuestos> CatPuestos { get; set; }
        public virtual DbSet<CatSubAreas> CatSubAreas { get; set; }
        public virtual DbSet<DetErrorLog> DetErrorLog { get; set; }
        public virtual DbSet<DetLogLogin> DetLogLogin { get; set; }
        public virtual DbSet<DetProcesoDetalle> DetProcesoDetalle { get; set; }
        public virtual DbSet<DetTiempoEspera> DetTiempoEspera { get; set; }
        public virtual DbSet<DetTipoProceso> DetTipoProceso { get; set; }
        public virtual DbSet<DetUsuarios> DetUsuarios { get; set; }
        public virtual DbSet<LogActividadEquipo> LogActividadEquipo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=DbContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatAreas>(entity =>
            {
                entity.HasKey(e => e.IdArea);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.NombreArea)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatConfiguracionesProcess>(entity =>
            {
                entity.HasKey(e => e.IdConfidguracion);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.Llave)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Valor)
                    .HasColumnName("valor")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatPais>(entity =>
            {
                entity.HasKey(e => e.IdPais);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBY");

                entity.Property(e => e.NombrePais)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatPerfiles>(entity =>
            {
                entity.HasKey(e => e.IdPerfil);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.NombrePerfil)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatPuestos>(entity =>
            {
                entity.HasKey(e => e.IdPuesto);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.NombrePuesto)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatSubAreas>(entity =>
            {
                entity.HasKey(e => e.IdSubArea);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.NombreSubArea)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DetErrorLog>(entity =>
            {
                entity.HasKey(e => e.IdError);

                entity.Property(e => e.DescError).IsUnicode(false);

                entity.Property(e => e.FechaError).HasColumnType("datetime");
            });

            modelBuilder.Entity<DetLogLogin>(entity =>
            {
                entity.HasKey(e => e.IdLogLogin);

                entity.Property(e => e.FechaLogin).HasColumnType("datetime");
            });

            modelBuilder.Entity<DetProcesoDetalle>(entity =>
            {
                entity.HasKey(e => e.IdProceso);

                entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.FechaProceso).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.NombreProceso)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NombreTab).IsUnicode(false);

                entity.Property(e => e.ProcessId)
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DetTiempoEspera>(entity =>
            {
                entity.HasKey(e => e.IdTiempo);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TimerName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DetTipoProceso>(entity =>
            {
                entity.HasKey(e => e.IdTipoProceso);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.NombreProceso)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TipoProceso)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DetUsuarios>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .HasColumnName("clave")
                    .IsUnicode(false);

                entity.Property(e => e.CodEmpleado)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Usuario)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogActividadEquipo>(entity =>
            {
                entity.HasKey(e => e.IdLogActividad);

                entity.Property(e => e.FechaFinal).HasColumnType("datetime");

                entity.Property(e => e.FechaIncial).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
