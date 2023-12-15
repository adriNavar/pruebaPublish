using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class INMLibreDeDeudaTemporalMapper : TablaTemporalMapper<INMLibreDeDeudaTemporal>
    {
        public INMLibreDeDeudaTemporalMapper()
            : base("INM_LIBRE_DEUDA")
        {
            HasKey(p => new { p.IdLibreDeuda, p.IdTramite });

            this.Property(a => a.IdLibreDeuda)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_LIBRE_DEUDA");

            this.Property(a => a.IdUnidadTributaria)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.FechaEmision)
                .IsRequired()
                .HasColumnName("FECHA_EMISION");

            this.Property(a => a.FechaVigencia)
                .IsRequired()
                .HasColumnName("FECHA_VIGENCIA");

            this.Property(a => a.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.IdEnteEmisor)
                .IsRequired()
                .HasColumnName("ID_ENTE_EMISOR");

            this.Property(a => a.NroCertificado)
                .HasColumnName("NRO_CERTIFICADO");

            this.Property(a => a.Superficie)
                .HasColumnName("SUPERFICIE");

            this.Property(a => a.Valuacion)
                .HasColumnName("VALUACION");

            this.Property(a => a.Deuda)
                .HasColumnName("DEUDA");

        }
    }
}