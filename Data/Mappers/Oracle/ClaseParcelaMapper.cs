using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ClaseParcelaMapper : EntityTypeConfiguration<ClaseParcela>
    {
        public ClaseParcelaMapper()
        {
            this.ToTable("INM_CLASE_PARCELA")
                .HasKey(cp => cp.ClaseParcelaID);

            this.Property(cp => cp.ClaseParcelaID)
                .HasColumnName("ID_CLASE_PARCELA")
                .IsRequired();

            this.Property(cp => cp.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(cp => cp.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(cp => cp.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
