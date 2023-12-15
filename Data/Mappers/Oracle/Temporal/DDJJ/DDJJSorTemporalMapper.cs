using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJSorTemporalMapper : TablaTemporalMapper<DDJJSorTemporal>
    {
        public DDJJSorTemporalMapper()
            : base("VAL_DDJJ_SOR")
        {
            HasKey(a => a.IdSor);

            Property(a => a.IdSor)
                .IsRequired()
                .HasColumnName("ID_DDJJ_SOR");

            Property(a => a.IdLocalidad)
                .HasColumnName("ID_LOCALIDAD");

            Property(a => a.IdCamino)
                .HasColumnName("ID_CAMINO");

            Property(a => a.DistanciaCamino)
                .HasColumnName("DISTANCIA_CAMINO");

            Property(a => a.DistanciaLocalidad)
                .HasColumnName("DISTANCIA_LOCALIDAD");

            Property(a => a.DistanciaEmbarque)
                .HasColumnName("DISTANCIA_EMBARQUE");

            Property(a => a.NumeroHabitantes)
                .HasColumnName("NRO_HABITANTES");

            Property(a => a.IdMensura)
                .HasColumnName("ID_MENSURA");

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


            HasOptional(a => a.Mensura)
                .WithMany()
                .HasForeignKey(a => new { a.IdMensura, a.IdTramite });

            HasMany(a => a.SoRCars)
                .WithRequired()
                .HasForeignKey(a => a.IdSor)
                .WillCascadeOnDelete();

            HasMany(a => a.Superficies)
                .WithRequired()
                .HasForeignKey(a => a.IdSor)
                .WillCascadeOnDelete();
        }
    }
}