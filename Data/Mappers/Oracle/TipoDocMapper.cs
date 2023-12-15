using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoDocMapper : EntityTypeConfiguration<TipoDoc>
    {
        public TipoDocMapper()
        {
            this.ToTable("INM_TIPO_DOC_IDENTIDAD")
                .HasKey(a => a.Id_Tipo_Doc_Ident);

            this.Property(a => a.Id_Tipo_Doc_Ident)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_DOC_IDENT");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

        }
    }
}
