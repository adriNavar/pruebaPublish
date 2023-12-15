using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MensuraDocumentoMapper : EntityTypeConfiguration<MensuraDocumento>
    {
        public MensuraDocumentoMapper()
        {
            ToTable("INM_MENSURA_DOCUMENTO");

            HasKey(p => p.IdMensuraDocumento);

            Property(p => p.IdMensuraDocumento)
                .HasColumnName("ID_MENSURA_DOCUMENTO")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdMensura)
                .HasColumnName("ID_MENSURA")
                .IsRequired();

            Property(p => p.IdDocumento)
                .HasColumnName("ID_DOCUMENTO")
                .IsRequired();



            this.Property(a => a.IdUsuarioAlta)
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(a => a.Mensura)
                .WithMany(p => p.MensurasDocumentos)
                .HasForeignKey(a => a.IdMensura);

            HasRequired(a => a.Documento)
                .WithMany()
                .HasForeignKey(a => a.IdDocumento);
        }
    }
}
