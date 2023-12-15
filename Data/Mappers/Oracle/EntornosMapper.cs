using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EntornosMapper : EntityTypeConfiguration<Entornos>
    {
        public EntornosMapper()
        {
            this.ToTable("GE_ENTORNO");

            this.Property(a => a.Id_Entorno)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ENTORNO");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");

            this.HasKey(a => a.Id_Entorno);

        }
    }
}
