using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class INMMejoraOtraCarTemporalMapper : TablaTemporalMapper<INMMejoraOtraCarTemporal>
    {
        public INMMejoraOtraCarTemporalMapper()
            : base("INM_MEJORAS_OTRAS_CAR")
        {
            HasKey(a => new { a.IdMejora, a.IdOtraCar });

            Property(a => a.IdOtraCar)
                .IsRequired()
                .HasColumnName("ID_OTRA_CAR");

            Property(a => a.IdMejora)
                .IsRequired()
                .HasColumnName("ID_MEJORA");

            Property(a => a.Valor)
                .HasColumnName("VALOR");

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


            HasRequired(a => a.OtraCar)
                .WithMany()
                .HasForeignKey(a => a.IdOtraCar);
        }

    }
}
