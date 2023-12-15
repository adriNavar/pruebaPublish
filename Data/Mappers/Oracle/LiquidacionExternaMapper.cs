using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LiquidacionExternaMapper : EntityTypeConfiguration<LiquidacionExterna>
    {
        public LiquidacionExternaMapper()
        {
            ToTable("VR3D100$TLW_INS$GEOSYS");

            HasKey(l => new { l.Expediente, l.Fecha, l.Padron });

            Property(l => l.Expediente)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("EXPEDIENTE");

            Property(l => l.Padron)
                .IsRequired()
                .HasColumnName("PADRON");

            Property(l => l.Importe)

                .HasColumnName("IMPORTE");

            Property(l => l.Fecha)
                .IsRequired()
                .HasColumnName("FECHA_LIQUIDACION");


        }
    }
}
