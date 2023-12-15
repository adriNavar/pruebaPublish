using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJDesignacionTemporalMapper : TablaTemporalMapper<DDJJDesignacionTemporal>
    {
        public DDJJDesignacionTemporalMapper()
            : base("VAL_DDJJ_DESIGNACION")
        {
            HasKey(a => a.IdDeclaracionJurada);

            Property(a => a.IdDeclaracionJurada)
                .IsRequired()
                .HasColumnName("ID_DDJJ");

            Property(a => a.IdTipoDesignador)
                .IsRequired()
                .HasColumnName("ID_TIPO_DESIGNADOR");

            Property(a => a.IdCalle)
                .HasColumnName("ID_CALLE");

            Property(a => a.Calle)
                .HasColumnName("CALLE");

            Property(a => a.Numero)
                .HasColumnName("NUMERO");

            Property(a => a.IdBarrio)
                .HasColumnName("ID_BARRIO");

            Property(a => a.Barrio)
                .HasColumnName("BARRIO");

            Property(a => a.IdLocalidad)
                .IsRequired()
                .HasColumnName("ID_LOCALIDAD");

            Property(a => a.Localidad)
                .IsRequired()
                .HasColumnName("LOCALIDAD");

            Property(a => a.IdDepartamento)
                .IsRequired()
                .HasColumnName("ID_DEPARTAMENTO");

            Property(a => a.Departamento)
                .IsRequired()
                .HasColumnName("DEPARTAMENTO");

            Property(a => a.IdParaje)
                .HasColumnName("ID_PARAJE");

            Property(a => a.Paraje)
                .HasColumnName("PARAJE");

            Property(a => a.IdSeccion)
                .HasColumnName("ID_SECCION");

            Property(a => a.Seccion)
                .HasColumnName("SECCION");

            Property(a => a.Chacra)
                .HasColumnName("CHACRA");

            Property(a => a.Quinta)
                .HasColumnName("QUINTA");

            Property(a => a.Fraccion)
                .HasColumnName("FRACCION");

            Property(a => a.IdManzana)
                .HasColumnName("ID_MANZANA");

            Property(a => a.Manzana)
                .HasColumnName("MANZANA");

            Property(a => a.Lote)
                .HasColumnName("LOTE");

            Property(a => a.CodigoPostal)
                .HasColumnName("CODIGO_POSTAL");

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
