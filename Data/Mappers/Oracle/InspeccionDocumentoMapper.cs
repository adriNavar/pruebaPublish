using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspeccionDocumentoMapper : EntityTypeConfiguration<InspeccionDocumento>
    {
        public InspeccionDocumentoMapper()
        {
            this.ToTable("INM_INSPECCION_DOCUMENTO");

            this.Property(a => a.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_INSPECCION_DOCUMENTO");
            this.Property(a => a.InspeccionID)
                .IsRequired()
                .HasColumnName("ID_INSPECCION");
            this.Property(a => a.id_documento)
                .IsRequired()
                .HasColumnName("ID_DOCUMENTO");

            this.HasKey(a => a.Id);

            this.HasRequired(a => a.documento).WithMany().HasForeignKey(a => a.id_documento);
        }
    }
}
