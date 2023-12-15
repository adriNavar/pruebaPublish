using GeoSit.Data.BusinessEntities.Documentos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DocumentoMapper : EntityTypeConfiguration<Documento>
    {
        public DocumentoMapper()
        {
            this.ToTable("DOC_DOCUMENTOS");
            this.Property(a => a.id_documento)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DOCUMENTO");
            this.Property(a => a.id_tipo_documento)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOCUMENTO");
            this.Property(a => a.fecha)
                .HasColumnName("FECHA");
            this.Property(a => a.descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.observaciones)
                .IsUnicode(false)
                .HasColumnName("OBSERVACIONES");
            this.Property(a => a.nombre_archivo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_ARCHIVO");
            this.Property(a => a.extension_archivo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EXTENSION_ARCHIVO");
            this.Property(a => a.id_usu_alta)
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.fecha_alta_1)
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.id_usu_modif)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.fecha_modif)
                .HasColumnName("FECHA_MODIF");
            this.Property(a => a.id_usu_baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.fecha_baja_1)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.contenido)
                .HasColumnName("CONTENIDO");
            this.Property(a => a.ruta)
                .HasColumnName("RUTA")
                .HasMaxLength(255);
            this.Property(a => a.atributos)
                .HasColumnName("ATRIBUTOS");


            /*Navegacion*/
            this.HasRequired(a => a.Tipo)
                .WithMany(a => a.Documentos)
                .HasForeignKey(d => d.id_tipo_documento);

            this.HasKey(a => a.id_documento);

        }
    }
}
