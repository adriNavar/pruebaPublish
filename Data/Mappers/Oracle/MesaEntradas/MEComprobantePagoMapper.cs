using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEComprobantePagoMapper : EntityTypeConfiguration<MEComprobantePago>
    {
        public MEComprobantePagoMapper()
        {
            this.ToTable("ME_COMPROBANTE_PAGO");

            this.HasKey(a => a.IdComprobantePago);

            this.Property(a => a.IdComprobantePago)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COMPROBANTE_PAGO");

            this.Property(a => a.IdTipoTasa)
                .HasColumnName("ID_TIPO_TASA");

            this.Property(a => a.IdTramite)
                .HasColumnName("ID_TRAMITE_DGR");

            this.Property(a => a.TipoTramiteDgr)
                .HasColumnName("TIPO_TRAMITE_DGR");

            this.Property(a => a.FechaVencimiento)
                .HasColumnName("FECHA_VENCIMIENTO");

            this.Property(a => a.FechaLiquidacion)
                .HasColumnName("FECHA_LIQUIDACION");

            this.Property(a => a.FechaPago)
                .HasColumnName("FECHA_PAGO");

            this.Property(a => a.MedioPago)
                .HasColumnName("MEDIO_PAGO");

            this.Property(a => a.Importe)
                .HasColumnName("IMPORTE");

            this.Property(a => a.EstadoPago)
                .HasColumnName("ESTADO_PAGO");

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
        }
    }
}
