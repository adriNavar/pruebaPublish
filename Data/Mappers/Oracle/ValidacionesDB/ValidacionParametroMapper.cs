using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ValidacionesDB
{
    public class ValidacionParametroMapper : EntityTypeConfiguration<ValidacionParametro>
    {
        public ValidacionParametroMapper()
        {
            ToTable("VL_VALIDACION_PARAMETRO")
                .HasKey(vlp => new { vlp.IdValidacion, vlp.Parametro });

            Property(a => a.IdValidacion)
                .IsRequired()
                .HasColumnName("ID_VALIDACION");

            Property(a => a.Parametro)
                .IsRequired()
                .HasColumnName("PARAMETRO");

            Property(a => a.Propiedad)
                .IsRequired()
                .HasColumnName("PROPIEDAD");

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
        }
    }
}
