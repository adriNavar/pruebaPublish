using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ValidacionesDB
{
    public class ValidacionSubtipoMapper : EntityTypeConfiguration<ValidacionSubtipo>
    {
        public ValidacionSubtipoMapper()
        {
            ToTable("VL_VALIDACION_SUBTIPO")
                .HasKey(vl => vl.IdValidacion);

            Property(a => a.IdValidacion)
                .IsRequired()
                .HasColumnName("ID_VALIDACION");

            Property(a => a.IdValidacionSubtipo)
                .IsRequired()
                .HasColumnName("ID_VALIDACION_SUBTIPO");

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