using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIRVbEuCoefEstadoMapper : EntityTypeConfiguration<VIRVbEuCoefEstado>
    {
        public VIRVbEuCoefEstadoMapper()
        {
            this.ToTable("VIR_VB_EU_COEF_ESTADO", "VIR_VALUACIONES")
                .HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsOptional();

            this.Property(p => p.Coeficiente)
                .HasColumnName("COEF")
                .IsOptional();
        }
    }
}
