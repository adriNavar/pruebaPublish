using GeoSit.Data.BusinessEntities.Documentos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoDocumentoMapper : EntityTypeConfiguration<TipoDocumento>
    {
        public TipoDocumentoMapper()
        {
            this.ToTable("DOC_TIPO_DOCUMENTO")
                .HasKey(p => p.TipoDocumentoId);

            this.Property(p => p.TipoDocumentoId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOCUMENTO");

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.Property(td => td.EsEditable)
               .IsRequired()
               .HasColumnName("ES_EDITABLE");

            this.Property(td => td.EsEliminable)
                .IsRequired()
                .HasColumnName("ES_ELIMINABLE");

            this.Property(td => td.Esquema)
                .IsRequired()
                .HasColumnName("ESQUEMA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasMany(p => p.Documentos)
                 .WithRequired(p => p.Tipo)
                 .HasForeignKey(k => k.id_tipo_documento);
        }

    }
}
