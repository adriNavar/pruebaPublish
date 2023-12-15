using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.Valuacion
{
    public class VALValuacionDecretoTemporalMapper : TablaTemporalMapper<VALValuacionDecretoTemporal>
    {
        public VALValuacionDecretoTemporalMapper()
            : base("VAL_VAL_DECRETOS")
        {
            HasKey(a => new { a.IdValuacion, a.IdDecreto });

            Property(a => a.IdValuacion)
                .IsRequired()
                .HasColumnName("ID_VALUACION");

            Property(a => a.IdDecreto)
                .IsRequired()
                .HasColumnName("ID_DECRETO");

            Property(a => a.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
