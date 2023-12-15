using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaCategoriaMapper: EntityTypeConfiguration<PlantillaCategoria>
    {
        public PlantillaCategoriaMapper()
        {
            ToTable("MP_PLANTILLA_CATEGORIA");

            HasKey(pc => pc.IdPlantillaCategoria);

            Property(pc => pc.IdPlantillaCategoria)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_CATEGORIA");

            Property(pc => pc.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
        }
    }
}
