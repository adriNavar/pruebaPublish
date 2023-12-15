using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class INMMejoraCaracteristicaTemporalMapper : TablaTemporalMapper<INMMejoraCaracteristicaTemporal>
    {
        public INMMejoraCaracteristicaTemporalMapper()
            : base("INM_MEJORAS_CARACTERISTICAS")
        {
            HasKey(a => new { a.IdMejora, a.IdCaracteristica });

            Property(a => a.IdMejora)
                .HasColumnName("ID_MEJORA");

            Property(a => a.IdCaracteristica)
                .HasColumnName("ID_CARACTERISTICA");

            Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");


            HasRequired(a => a.Caracteristica)
                .WithMany()
                .HasForeignKey(a => a.IdCaracteristica);
        }

    }
}
