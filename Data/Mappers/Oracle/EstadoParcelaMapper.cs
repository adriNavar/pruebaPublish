using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadoParcelaMapper : EntityTypeConfiguration<EstadoParcela>
    {
        public EstadoParcelaMapper()
        {
            this.ToTable("INM_ESTADO_PARCELA")
                .HasKey(ep => ep.EstadoParcelaID);

            this.Property(ep => ep.EstadoParcelaID)
                .HasColumnName("ID_ESTADO_PARCELA")
                .IsRequired();

            this.Property(ep => ep.Descripcion)
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
