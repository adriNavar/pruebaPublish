using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class VALAptCarTemporalMapper : TablaTemporalMapper<VALAptCarTemporal>
    {
        public VALAptCarTemporalMapper()
            : base("VAL_APT_CAR")
        {
            HasKey(a => a.IdAptCar);

            Property(a => a.IdAptCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_APT_CAR");

            Property(a => a.IdSorCar)
                .IsRequired()
                .HasColumnName("ID_SOR_CAR");

            Property(a => a.IdAptitud)
                .IsRequired()
                .HasColumnName("ID_APTITUD");

            Property(a => a.Puntaje)
                .HasColumnName("PUNTAJE");

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


            HasRequired(a => a.SorCaracteristica)
                .WithMany()
                .HasForeignKey(a => a.IdSorCar);

            HasRequired(a => a.Aptitud)
                .WithMany()
                .HasForeignKey(a => a.IdAptitud);


        }
    }
}
