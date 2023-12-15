using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.ValidacionesDB
{
    public class ValidacionGrupoFuncionMapper : EntityTypeConfiguration<ValidacionGrupoFuncion>
    {
        public ValidacionGrupoFuncionMapper()
        {
            ToTable("VL_GRUPO_FUNCION")
                .HasKey(x => new { x.IdGrupo, x.IdFuncion });

            Property(a => a.IdGrupo)
                .IsRequired()
                .HasColumnName("ID_GRUPO");

            Property(a => a.IdFuncion)
                .IsRequired()
                .HasColumnName("ID_FUNCION");
        }
    }
}
