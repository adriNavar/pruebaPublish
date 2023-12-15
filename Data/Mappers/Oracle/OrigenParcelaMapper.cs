using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class OrigenParcelaMapper : EntityTypeConfiguration<OrigenParcela>
    {
        public OrigenParcelaMapper()
        {
            this.ToTable("INM_ORIGEN_PARCELA");

            this.Property(op => op.OrigenParcelaID)
                .HasColumnName("ID_ORIGEN_PARCELA")
                .IsRequired();

            this.Property(op => op.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired()
                .HasMaxLength(50);

            this.HasKey(op => op.OrigenParcelaID);
        }
    }
}
