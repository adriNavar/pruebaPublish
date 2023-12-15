using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEDesgloseMapper : EntityTypeConfiguration<MEDesglose>
    {
        public MEDesgloseMapper()
        {
            this.ToTable("ME_DESGLOSE");

            this.HasKey(a => a.IdDesglose);

            this.Property(a => a.IdDesglose)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DESGLOSE");

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.IdDesgloseDestino)
                .IsRequired()
                .HasColumnName("ID_DESGLOSE_DESTINO");

            this.Property(a => a.FolioDesde)
                .IsRequired()
                .HasColumnName("FOLIO_DESDE");

            this.Property(a => a.FolioHasta)
                .IsRequired()
                .HasColumnName("FOLIO_HASTA");

            this.Property(a => a.FolioCant)
                .IsRequired()
                .HasColumnName("FOLIO_CANT");

            this.Property(a => a.Observaciones)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("OBSERVACIONES");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("USUARIO_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.DesgloseDestino)
                .WithMany()
                .HasForeignKey(a => a.IdDesgloseDestino);

            //this.HasRequired(a => a.Usuario_Alta)
            //    .WithMany()
            //    .HasForeignKey(a => a.UsuarioAlta);

            //this.HasRequired(a => a.Tramite)
            //    .WithMany(t => t.TramiteRequisitos)
            //    .HasForeignKey(a => a.IdTramite);

        }
    }
}
