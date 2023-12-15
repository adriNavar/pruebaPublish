using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class MensuraTemporalMapper : TablaTemporalMapper<MensuraTemporal>
    {
        public MensuraTemporalMapper() 
            : base("INM_MENSURA")
        {
            HasKey(p => new { p.IdMensura, p.IdTramite });

            Property(p => p.IdMensura)
                .HasColumnName("ID_MENSURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdTipoMensura)
                .HasColumnName("ID_TIPO_MENSURA")
                .IsRequired();

            Property(p => p.IdEstadoMensura)
                .HasColumnName("ID_ESTADO_MENSURA")
                .IsRequired();

            Property(p => p.Numero)
                .HasColumnName("NUMERO");

            Property(p => p.Letra)
                .HasColumnName("LETRA");

            Property(p => p.FechaPresentacion)
                .HasColumnName("FECHA_PRESENTACION");

            Property(p => p.FechaAprobacion)
                .HasColumnName("FECHA_APROBACION");

            Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            Property(p => p.MensurasRelacionadas)
                .HasColumnName("MENSURAS_RELACIONADAS");

            Property(p => p.Observaciones)
                .HasColumnName("OBSERVACIONES");

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
        }
    }
}