using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParcelaDocumentoMapper : EntityTypeConfiguration<ParcelaDocumento>
    {
        public ParcelaDocumentoMapper()
        {
            ToTable("INM_PARCELA_DOCUMENTOS");
            HasKey(pd => pd.ParcelaDocumentoID);

            Property(pd => pd.ParcelaDocumentoID)
                .HasColumnName("ID_PARCELA_DOCUMENTOS")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(pd => pd.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            Property(pd => pd.DocumentoID)
                .HasColumnName("ID_DOCUMENTO")
                .IsOptional();

            Property(pd => pd.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(pd => pd.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(pd => pd.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            Property(pd => pd.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            Property(pd => pd.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(pd => pd.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            HasRequired(pd => pd.Parcela)
                .WithMany(p => p.ParcelaDocumentos)
                .HasForeignKey(pd => pd.ParcelaID);

            HasRequired(pd => pd.Documento)
                .WithMany()
                .HasForeignKey(pd => pd.DocumentoID);

        }
    }
}
