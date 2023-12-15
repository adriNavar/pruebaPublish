using GeoSit.Data.BusinessEntities.SubSistemaWeb;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class WebLinksMapper : EntityTypeConfiguration<WebLinks> 
    {
        public WebLinksMapper ()
        {
            this.ToTable("WEB_LINKS");

            this.Property(a => a.idLink)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_LINK");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.URL)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("URL");

            this.HasKey(a => a.idLink);

        }

    }
}
