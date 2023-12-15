using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEMovimientoMapper : EntityTypeConfiguration<MEMovimiento>
    {
        public MEMovimientoMapper()
        {
            this.ToTable("ME_MOVIMIENTO");

            this.HasKey(a => a.IdMovimiento);

            this.Property(a => a.IdMovimiento)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_MOVIMIENTO");

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.IdTipoMovimiento)
                .IsRequired()
                .HasColumnName("ID_TIPO_MOVIMIENTO");

            this.Property(a => a.IdSectorOrigen)
                .IsRequired()
                .HasColumnName("ID_SECTOR_ORIGEN");

            this.Property(a => a.IdSectorDestino)
                .IsRequired()
                .HasColumnName("ID_SECTOR_DESTINO");

            this.Property(a => a.IdRemito)
                .HasColumnName("ID_REMITO");

            this.Property(a => a.IdEstado)
                .IsRequired()
                .HasColumnName("ID_ESTADO");

            this.Property(a => a.Observacion)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("OBSERVACION");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioRecep)
                .HasColumnName("USUARIO_RECEP");

            this.Property(a => a.FechaRecep)
                .HasColumnName("FECHA_RECEP");

            this.HasRequired(a => a.TipoMovimiento)
                .WithMany()
                .HasForeignKey(a => a.IdTipoMovimiento);

            this.HasRequired(a => a.SectorOrigen)
                .WithMany()
                .HasForeignKey(a => a.IdSectorOrigen);

            this.HasRequired(a => a.SectorDestino)
                .WithMany()
                .HasForeignKey(a => a.IdSectorDestino);

            this.HasRequired(a => a.Tramite)
                .WithMany(t => t.Movimientos)
                .HasForeignKey(a => a.IdTramite);

            this.HasRequired(a => a.Estado)
                .WithMany()
                .HasForeignKey(a => a.IdEstado);

            this.HasRequired(a => a.Usuario_Alta)
                .WithMany()
                .HasForeignKey(a => a.UsuarioAlta);
        }
    }
}
