using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ProvinciaMapper : EntityTypeConfiguration<Provincia>
    {
        public ProvinciaMapper()
        {
            this.ToTable("INM_PROVINCIA");

            this.Property(p => p.ProvinciaId)
                .IsRequired()
                .HasColumnName("ID_PROVINCIA");

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.HasKey(p => p.ProvinciaId);
        }
    }
}
