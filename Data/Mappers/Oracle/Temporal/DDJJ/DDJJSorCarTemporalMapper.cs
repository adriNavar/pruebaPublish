using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJSorCarTemporalMapper : TablaTemporalMapper<DDJJSorCarTemporal>
    {
        public DDJJSorCarTemporalMapper()
            : base("VAL_DDJJ_SOR_CAR")
        {
            HasKey(a => new { a.IdSor, a.IdAptCar });

            Property(a => a.IdSor)
                .IsRequired()
                .HasColumnName("ID_DDJJ_SOR");

            Property(a => a.IdAptCar)
                .HasColumnName("ID_APT_CAR");

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


            HasRequired(a => a.AptCar)
                .WithMany()
                .HasForeignKey(a => a.IdAptCar);
        }
    }
}
