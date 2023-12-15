using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class ParcelaMensuraTemporalMapper : TablaTemporalMapper<ParcelaMensuraTemporal>
    {
        public ParcelaMensuraTemporalMapper()
            : base("INM_PARCELA_MENSURA")
        {
            HasKey(p => p.IdParcelaMensura);

            Property(p => p.IdParcelaMensura)
                .HasColumnName("ID_PARCELA_MENSURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdParcela)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            Property(p => p.IdMensura)
                .HasColumnName("ID_MENSURA")
                .IsRequired();

            Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");

            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(a => a.Mensura)
                .WithMany()
                .HasForeignKey(a => new { a.IdMensura, a.IdTramite });
        }
    }
}
