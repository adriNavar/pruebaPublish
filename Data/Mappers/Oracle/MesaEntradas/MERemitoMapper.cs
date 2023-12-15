using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MERemitoMapper : EntityTypeConfiguration<MERemito>
    {
        public MERemitoMapper()
        {
            this.ToTable("ME_REMITO");

            this.HasKey(a => a.IdRemito);

            this.Property(a => a.IdRemito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_REMITO");

            this.Property(a => a.Numero)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NUMERO");

            this.Property(a => a.IdSectorOrigen)
                .IsRequired()
                .HasColumnName("ID_SECTOR_ORIGEN");

            this.Property(a => a.IdSectorDestino)
                .IsRequired()
                .HasColumnName("ID_SECTOR_DESTINO");

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

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Receptor)
                .WithMany()
                .HasForeignKey(a => a.UsuarioRecep);

            this.HasRequired(a => a.SectorOrigen)
                .WithMany()
                .HasForeignKey(a => a.IdSectorOrigen);

            this.HasRequired(a => a.SectorDestino)
                .WithMany()
                .HasForeignKey(a => a.IdSectorDestino);

            this.HasMany(a => a.Movimientos)
                .WithOptional(m => m.Remito)
                .HasForeignKey(a => a.IdRemito);

        }
    }
}
