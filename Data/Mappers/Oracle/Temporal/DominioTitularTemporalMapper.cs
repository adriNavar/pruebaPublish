using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class DominioTitularTemporalMapper : TablaTemporalMapper<DominioTitularTemporal>
    {
        public DominioTitularTemporalMapper()
            :base("INM_DOMINIO_TITULAR")
        {
            HasKey(dt => new { dt.DominioID, dt.IdTramite, dt.PersonaID });

            Property(dt => dt.DominioID)
                .HasColumnName("ID_DOMINIO")
                .IsRequired();

            Property(dt => dt.PersonaID)
                .HasColumnName("ID_PERSONA")
                .IsRequired();

            Property(dt => dt.TipoTitularidadID)
                .HasColumnName("ID_TIPO_TITULARIDAD")
                .IsRequired();

            Property(dt => dt.PorcientoCopropiedad)
                .HasColumnName("PORCIENTO_COPROPIEDAD")
                .IsRequired();

            Property(dt => dt.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(dt => dt.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(dt => dt.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(dt => dt.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(dt => dt.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(dt => dt.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            //HasRequired(dt => dt.Dominio)
            //    .WithMany(d => d.Titulares)
            //    .HasForeignKey(dt => dt.DominioID);

            //HasRequired(dt => dt.Persona)
            //    .WithMany()
            //    .HasForeignKey(dt => dt.PersonaID);
        }
    }
}
