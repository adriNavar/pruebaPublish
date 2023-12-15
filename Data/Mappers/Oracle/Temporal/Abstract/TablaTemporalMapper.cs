using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.Temporal.Abstract
{
    public abstract class TablaTemporalMapper<T> : EntityTypeConfiguration<T> where T : class, ITemporalTramite
    {
        protected TablaTemporalMapper(string tabla)
        {
            ToTable(tabla, "TEMPORAL");

            Property(t => t.IdTramite)
                .HasColumnName("ID_TRAMITE")
                .IsRequired();
        }
    }
}
