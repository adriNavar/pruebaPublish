using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class EspacioPublicoTemporalMapper : TablaTemporalMapper<EspacioPublicoTemporal>
    {
        public EspacioPublicoTemporalMapper() 
            : base("INM_ESPACIO_PUBLICO")
        {
            HasKey(p => new { p.EspacioPublicoID, p.IdTramite });

            Property(p => p.EspacioPublicoID)
                .HasColumnName("ID_ESPACIO_PUBLICO")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.ParcelaID)
               .HasColumnName("ID_PARCELA")
               .IsRequired();

            Property(p => p.IdTramite)
                .HasColumnName("ID_TRAMITE")
                .IsRequired();

            Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(p => p.Superficie)
                .HasColumnName("SUPERFICIE")
                .IsRequired();

            Property(p => p.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(p => p.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(p => p.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

        }
    }
}
