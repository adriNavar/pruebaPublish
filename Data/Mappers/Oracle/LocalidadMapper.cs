using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LocalidadMapper : EntityTypeConfiguration<Localidad>
    {
        public LocalidadMapper()
        {
            this.ToTable("INM_LOCALIDAD");

            this.Property(p => p.LocalidadId)
                .IsRequired()
                .HasColumnName("ID_LOCALIDAD");

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.HasKey(p => p.LocalidadId);
        }
    }
}
