using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ConfiguracionFiltroGraficoMapper : EntityTypeConfiguration<ConfiguracionFiltroGrafico>
    {
        public ConfiguracionFiltroGraficoMapper()
        {
            this.ToTable("MT_CONFIG_FILTRO_GRAF");

            this.Property(a => a.ConfiguracionFiltroGraficoId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CONFIG_FILTRO_GRAF");
            this.Property(a => a.FiltroId)
                .IsRequired()
                .HasColumnName("ID_CONFIG_FILTRO");
            this.Property(a => a.Geometry)
                .IsRequired()
                .HasColumnName("GEOMETRY");


            this.Ignore(a => a.Coordenadas);
            this.Ignore(a => a.sGeometry);
            this.Ignore(a => a.Geom);
            this.Ignore(a => a.WKT);

            this.HasKey(a => a.ConfiguracionFiltroGraficoId);

            //Relationship with ConfiguracionFiltro
            HasRequired(a => a.ConfiguracionFiltro)
                .WithMany(cf => cf.ConfiguracionesFiltroGrafico)
                .HasForeignKey(a => a.FiltroId);
        }
    }
}
