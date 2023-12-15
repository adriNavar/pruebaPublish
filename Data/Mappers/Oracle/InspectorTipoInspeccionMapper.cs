using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspectorTipoInspeccionMapper : EntityTypeConfiguration<InspectorTipoInspeccion>
    {
        public InspectorTipoInspeccionMapper()
        {
            this.ToTable("INM_ROL_INSPECTOR");

            this.Property(a => a.InspectorTipoInspeccionID)
                .IsRequired()
                .HasColumnName("ID_ROL_INSPECTOR");
            this.Property(a => a.InspectorID)
                .IsRequired()
                .HasColumnName("ID_INSPECTOR");

            this.Property(a => a.TipoInspeccionID)
                .IsRequired()
                .HasColumnName("ID_TIPO_INSPECCION");

        }
    }
}
