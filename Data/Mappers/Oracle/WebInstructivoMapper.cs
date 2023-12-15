using GeoSit.Data.BusinessEntities.SubSistemaWeb ;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class WebInstructivoMapper : EntityTypeConfiguration<WebInstructivo>
    {
        public  WebInstructivoMapper ()
        {
            this.ToTable("WEB_INSTRUCTIVO");

            this.Property(a => a.IdInstructivo )
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_INSTRUCTIVO");

            this.Property(a => a.Seccion)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("SECCION");
            
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.URL)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("URL");

            this.Property(a => a.IdFuncion)
                .HasColumnName("ID_FUNCION");

            this.HasKey(a => a.IdInstructivo);
        }
            
    }
}
