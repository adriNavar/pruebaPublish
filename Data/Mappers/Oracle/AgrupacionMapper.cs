using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AgrupacionMapper : EntityTypeConfiguration<Agrupacion>
    {
        public AgrupacionMapper()
        {

            this.ToTable("MT_AGRUPACION");

            this.Property(a => a.AgrupacionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_AGRUPACION");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");

            this.HasKey(a => a.AgrupacionId);
        }
    }
}
