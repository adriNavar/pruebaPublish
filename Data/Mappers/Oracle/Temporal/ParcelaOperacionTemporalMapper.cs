using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class ParcelaOperacionTemporalMapper : TablaTemporalMapper<ParcelaOperacionTemporal>
    {
        public ParcelaOperacionTemporalMapper()
            : base("INM_PARCELA_OPERACION")
        {
            HasKey(po => po.ParcelaOperacionID);

            Property(po => po.ParcelaOperacionID)
                .HasColumnName("ID_PARCELA_OPERACION")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(po => po.TipoOperacionID)
                .HasColumnName("ID_TIPO_OPERACION")
                .IsRequired();

            Property(po => po.FechaOperacion)
                .HasColumnName("FECHA_OPERACION")
                .IsOptional();

            Property(po => po.ParcelaOrigenID)
                .HasColumnName("ID_PARCELA_ORIGEN")
                .IsOptional();

            Property(po => po.ParcelaDestinoID)
                .HasColumnName("ID_PARCELA_DESTINO")
                .IsRequired();

            Property(po => po.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(po => po.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(po => po.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(po => po.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(po => po.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(po => po.FechaBajaID)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();
        }
    }
}
