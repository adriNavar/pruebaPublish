using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJDominioTitularTemporalMapper : TablaTemporalMapper<DDJJDominioTitularTemporal>
    {
        public DDJJDominioTitularTemporalMapper()
            : base("VAL_DDJJ_DOMINIO_TITULAR")
        {
            HasKey(a => a.IdDominioTitular);

            Property(a => a.IdDominioTitular)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_DOMINIO_TITULAR");

            Property(a => a.IdPersona)
                .IsRequired()
                .HasColumnName("ID_PERSONA");

            Property(a => a.IdTipoTitularidad)
                .IsRequired()
                .HasColumnName("ID_TIPO_TITULARIDAD");

            Property(a => a.IdDominio)
                .IsRequired()
                .HasColumnName("ID_DDJJ_DOMINIO");

            Property(a => a.PorcientoCopropiedad)
                .IsRequired()
                .HasColumnName("PORCIENTO_COPROPIEDAD");

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

            HasRequired(a => a.TipoTitularidad)
                .WithMany()
                .HasForeignKey(a => a.IdTipoTitularidad);

            HasMany(a => a.PersonaDomicilios)
                .WithOptional()
                .HasForeignKey(a => a.IdDominioTitular);
        }
    }
}
