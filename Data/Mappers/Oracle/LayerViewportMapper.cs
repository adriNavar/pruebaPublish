using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LayerViewportMapper : EntityTypeConfiguration<LayerViewport>
    {
        public LayerViewportMapper()
        {
            //Table mapping
            ToTable("MP_LAYER_VIEWPORT");

            //Primary Key
            HasKey(p => p.IdLayerViewport);

            //Properties
            Property(p => p.IdLayerViewport)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_LAYER_VIEWPORT");

            Property(p => p.IdPlantillaViewport)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_VIEWPORT");

            Property(p => p.IdLayer)
                .IsRequired()
                .HasColumnName("ID_LAYER");

            Property(p => p.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.UsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(p => p.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            /*//Relationship with Plantilla
            HasRequired(l => l.PlantillaViewport)
                .WithMany(p => p.layersViewport)
                .HasForeignKey(l => l.IdLayerViewport);*/
        }
    }
}
