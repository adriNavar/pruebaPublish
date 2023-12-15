using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class DesignacionTemporalMapper : TablaTemporalMapper<DesignacionTemporal>
    {
        public DesignacionTemporalMapper()
            : base("INM_DESIGNACION")
        {
            HasKey(a => a.IdDesignacion);

            Property(a => a.IdDesignacion)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DESIGNACION");

            Property(a => a.IdTipoDesignador)
                .HasColumnName("ID_TIPO_DESIGNADOR");

            Property(a => a.IdParcela)
                .HasColumnName("ID_PARCELA");

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
                .HasColumnName("ID_LOCALIDAD");

            Property(a => a.Localidad)
                .HasColumnName("LOCALIDAD");

            Property(a => a.IdDepartamento)
                .HasColumnName("ID_DEPARTAMENTO");

            Property(a => a.Departamento)
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

            HasRequired(n => n.Parcela)
                .WithMany()
                .HasForeignKey(n => new { n.IdParcela, n.IdTramite });
        }
    }
}
