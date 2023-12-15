using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PaisMapper : EntityTypeConfiguration<Pais>
    {
        public PaisMapper() {
            this.ToTable("INM_PAIS");

            this.Property(p => p.PaisId)
                .IsRequired()
                .HasColumnName("ID_PAIS");

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.HasKey(p => p.PaisId);
        }
    }
}
