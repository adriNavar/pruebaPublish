using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class UnidadTributariaPersonaTemporalMapper : TablaTemporalMapper<UnidadTributariaPersonaTemporal>
    {
        public UnidadTributariaPersonaTemporalMapper()
            : base("INM_UT_PERSONA")
        {
            HasKey(utp => new { utp.UnidadTributariaID, utp.PersonaID });

            Property(utp => utp.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            Property(utp => utp.PersonaID)
                .HasColumnName("ID_PERSONA")
                .IsRequired();

            Property(utp => utp.TipoPersonaID)
                .HasColumnName("ID_TIPO_PERSONA")
                .IsRequired();

            Property(utp => utp.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA");

            Property(utp => utp.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(utp => utp.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF");

            Property(utp => utp.FechaModificacion)
                .HasColumnName("FECHA_MODIF");

            Property(utp => utp.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(utp => utp.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(utp => utp.CodSistemaTributario)
                .HasColumnName("CODIGO_SISTEMA_TRIBUTARIO")
                .IsOptional();


            HasRequired(utp => utp.UnidadTributaria)
                .WithMany()
                .HasForeignKey(utp => new { utp.UnidadTributariaID, utp.IdTramite });
        }
    }
}
