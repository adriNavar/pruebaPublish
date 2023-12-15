using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaDocumentoMapper : EntityTypeConfiguration<ActaDocumento>
    {
        public ActaDocumentoMapper()
        {
            this.ToTable("OP_ACTA_DOCUMENTOS");

            this.Property(a => a.ActaID)
                .IsRequired()
                .HasColumnName("ID_ACTA");
            this.Property(a => a.id_documento)
                .IsRequired()
                .HasColumnName("ID_DOCUMENTO");

            this.HasKey(a => new { a.ActaID, a.id_documento });

            this.HasRequired(a => a.documento).WithMany().HasForeignKey(a => a.id_documento);

            HasRequired(a => a.Acta)
                .WithMany()
                .HasForeignKey(a => a.ActaID);
        }
    }
}
