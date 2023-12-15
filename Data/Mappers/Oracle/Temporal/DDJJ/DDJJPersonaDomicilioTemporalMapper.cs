using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJPersonaDomicilioTemporalMapper : TablaTemporalMapper<DDJJPersonaDomicilioTemporal>
    {
        public DDJJPersonaDomicilioTemporalMapper()
            : base("VAL_DDJJ_PERSONA_DOMICILIO")
        {
            HasKey(a => new { a.IdDominioTitular, a.IdDomicilio });

            Property(a => a.IdDominioTitular)
                .IsRequired()
                .HasColumnName("ID_DDJJ_DOMINIO_TITULAR");

            Property(a => a.IdDomicilio)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");

            Property(a => a.IdTipoDomicilio)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOMICILIO");

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

            HasRequired(a => a.Domicilio)
                .WithMany()
                .HasForeignKey(a => a.IdDomicilio);
            
            HasRequired(a => a.TipoDomicilio)
                .WithMany()
                .HasForeignKey(a => a.IdTipoDomicilio);
        }
    }
}
