using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class METramiteDocumentoMapper : EntityTypeConfiguration<METramiteDocumento>
    {
        public METramiteDocumentoMapper()
        {
            this.ToTable("ME_TRAMITE_DOCUMENTO");

            this.HasKey(a => a.IdTramiteDocumento);

            this.Property(a => a.IdTramiteDocumento)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_DOCUMENTO");

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.id_documento)
                .IsRequired()
                .HasColumnName("ID_DOCUMENTO");

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

            this.Property(a => a.FechaAprobacion)
                .HasColumnName("FECHA_APROBACION");

            this.HasRequired(a => a.Documento)
                .WithMany()
                .HasForeignKey(a => a.id_documento);

            //this.HasRequired(a => a.Tramite)
            //    .WithMany(t => t.TramiteDocumentos)
            //    .HasForeignKey(a => a.IdTramite);

            this.HasRequired(a => a.Usuario_Alta)
                .WithMany()
                .HasForeignKey(a => a.UsuarioAlta);

        }
    }
}
