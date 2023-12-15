using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoUnidadTributariaMapper : EntityTypeConfiguration<TipoUnidadTributaria>
    {
        public TipoUnidadTributariaMapper()
        {
            this.ToTable("INM_TIPO_UT")
                .HasKey(tp => tp.TipoUnidadTributariaID);

            this.Property(tp => tp.TipoUnidadTributariaID)
                .HasColumnName("ID_TIPO_UT")
                .IsRequired();

            this.Property(tp => tp.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
