using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class MensuraRelacionadaTemporalMapper : TablaTemporalMapper<MensuraRelacionadaTemporal>
    {
        public MensuraRelacionadaTemporalMapper() 
            : base("INM_MENSURA_RELACIONADA")
        {
            HasKey(p => new { p.IdMensuraDestino, p.IdMensuraOrigen, p.IdTramite });

            Property(p => p.IdMensuraDestino)
                .HasColumnName("ID_MENSURA_DESTINO")
                .IsRequired();

            Property(p => p.IdMensuraOrigen)
                .HasColumnName("ID_MENSURA_ORIGEN")
                .IsRequired();

            Property(a => a.IdUsuarioAlta)
                .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");

            Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(a => a.MensuraDestino)
                .WithMany()
                .HasForeignKey(a => new { a.IdMensuraDestino, a.IdTramite });
        }
    }
}