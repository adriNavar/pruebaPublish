using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaDocumentoMapper : EntityTypeConfiguration<UnidadTributariaDocumento>
    {
        public UnidadTributariaDocumentoMapper()
        {
            ToTable("INM_UT_DOCUMENTOS");
            HasKey(p => p.UnidadTributariaDocID);

            Property(utd => utd.UnidadTributariaDocID)
                .HasColumnName("ID_UT_DOCUMENTOS")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(utd => utd.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            Property(utd => utd.DocumentoID)
                .HasColumnName("ID_DOCUMENTO")
                .IsOptional();

            Property(utd => utd.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            Property(utd => utd.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            Property(utd => utd.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            Property(utd => utd.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            Property(utd => utd.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(utd => utd.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            HasRequired(p => p.UnidadTributaria)
                .WithMany(p=> p.UTDocumentos)
                .HasForeignKey(p => p.UnidadTributariaID);

            HasRequired(p => p.Documento)
                .WithMany()
                .HasForeignKey(d => d.DocumentoID);
        }
    }
}
