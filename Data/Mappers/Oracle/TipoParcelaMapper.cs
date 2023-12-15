using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoParcelaMapper : EntityTypeConfiguration<TipoParcela>
    {
        public TipoParcelaMapper()
        {
            this.ToTable("INM_TIPO_PARCELA")
                .HasKey(tp => tp.TipoParcelaID);

            this.Property(tp => tp.TipoParcelaID)
                .HasColumnName("ID_TIPO_PARCELA")
                .IsRequired();

            this.Property(tp => tp.Descripcion)
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
