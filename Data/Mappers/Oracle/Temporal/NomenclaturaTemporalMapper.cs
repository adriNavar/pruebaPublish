using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class NomenclaturaTemporalMapper : TablaTemporalMapper<NomenclaturaTemporal>
    {
        public NomenclaturaTemporalMapper()
            : base("INM_NOMENCLATURA")
        {
            HasKey(n => n.NomenclaturaID);

            Property(n => n.NomenclaturaID)
                .HasColumnName("ID_NOMENCLATURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(n => n.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            Property(n => n.Nombre)
                .HasColumnName("NOMENCLATURA")
                .IsRequired();

            Property(n => n.TipoNomenclaturaID)
                .HasColumnName("ID_TIPO_NOMENCLATURA")
                .IsRequired();

            Property(n => n.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(n => n.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            Property(n => n.FechaModificacion)
                .HasColumnName("FECHA_MODIF");

            Property(n => n.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA");

            Property(n => n.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA");

            Property(n => n.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF");

            HasRequired(n => n.Parcela)
                .WithMany()
                .HasForeignKey(n => new { n.ParcelaID, n.IdTramite });
        }
    }
}
