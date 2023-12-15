using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ValidacionesDB
{
    public class ValidacionFuncionMapper : EntityTypeConfiguration<ValidacionFuncion>
    {
        public ValidacionFuncionMapper()
        {
            ToTable("VL_VALIDACION_FUNCION")
                .HasKey(vlf => new { vlf.IdValidacion, vlf.IdFuncion });

            Property(a => a.IdValidacion)
                .IsRequired()
                .HasColumnName("ID_VALIDACION");

            Property(a => a.IdFuncion)
                .IsRequired()
                .HasColumnName("ID_FUNCION");

            Property(a => a.IdTipoMensaje)
                .IsRequired()
                .HasColumnName("ID_TIPO_MENSAJE");

            Property(a => a.Activa)
                .IsRequired()
                .HasColumnName("ACTIVA");

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