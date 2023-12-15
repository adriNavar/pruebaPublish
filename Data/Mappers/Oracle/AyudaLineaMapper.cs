using GeoSit.Data.BusinessEntities.SubSistemaWeb ;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AyudaLineaMapper : EntityTypeConfiguration<AyudaLinea>
    {
        public  AyudaLineaMapper ()
        {
            this.ToTable("WEB_AYUDA");

            this.Property(a => a.IdAyuda )
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_AYUDA");

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

            this.Property(a => a.IdTipo)
                .HasColumnName("ID_TIPO");

            this.Property(a => a.Orden)
                .IsRequired()
                .HasColumnName("ORDEN");

            this.HasKey(a => a.IdAyuda);
        }
            
    }
}
