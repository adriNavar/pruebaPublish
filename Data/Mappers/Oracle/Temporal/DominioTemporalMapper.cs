using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class DominioTemporalMapper : TablaTemporalMapper<DominioTemporal>
    {
        public DominioTemporalMapper()
            : base("INM_DOMINIO")
        {
            HasKey(d => new { d.DominioID, d.IdTramite });

            Property(d => d.DominioID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DOMINIO");

            Property(d => d.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            Property(d => d.TipoInscripcionID)
                .HasColumnName("ID_TIPO_INSCRIPCION");

            Property(d => d.Inscripcion)
                .HasColumnName("INSCRIPCION");

            Property(d => d.Fecha)
                .HasColumnName("FECHA");

            Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(a => a.IdUsuarioAlta)
                .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaModif)
                 .HasColumnName("FECHA_MODIF");

            Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            HasMany(a => a.Titulares)
                .WithRequired()
                .HasForeignKey(a => new { a.DominioID, a.IdTramite });
        }
    }
}