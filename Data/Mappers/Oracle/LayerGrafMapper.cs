using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LayerGrafMapper : EntityTypeConfiguration<LayerGraf>
    {
        public LayerGrafMapper()
        {
            ToTable("CT_PARCELA");

            HasKey(lg => lg.FeatId);

            Property(lg => lg.FeatId)
                .IsRequired()
                .HasColumnName("FEATID");

            Property(lg => lg.Nombre)
                .IsOptional()
                .HasColumnName("NOMBRE_EST");

            Ignore(lg => lg.Geom);            
        }
    }
}
