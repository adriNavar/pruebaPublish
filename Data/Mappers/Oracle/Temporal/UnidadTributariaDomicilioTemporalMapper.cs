using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class UnidadTributariaDomicilioTemporalMapper : TablaTemporalMapper<UnidadTributariaDomicilioTemporal>
    {
        public UnidadTributariaDomicilioTemporalMapper()
            : base("INM_UT_DOMICILIO")
        {
            HasKey(utd => new { utd.DomicilioID, utd.UnidadTributariaID });

            Property(utd => utd.DomicilioID)
                .HasColumnName("ID_DOMICILIO");

            Property(utd => utd.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            Property(utd => utd.TipoDomicilioID)
                .HasColumnName("ID_TIPO_DOMICILIO");

            Property(utd => utd.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA");

            Property(utd => utd.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(utd => utd.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF");

            Property(utd => utd.FechaModificacion)
                .HasColumnName("FECHA_MODIF");

            Property(utd => utd.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA");

            Property(utd => utd.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(utd => utd.UnidadTributaria)
                .WithMany()
                .HasForeignKey(utd => new { utd.UnidadTributariaID, utd.IdTramite });
        }
    }
}
