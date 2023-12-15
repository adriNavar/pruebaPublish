using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PloteoFrecuenteEspecialMapper : EntityTypeConfiguration<PloteoFrecuenteEspecial>
    {
        public PloteoFrecuenteEspecialMapper()
        {
            //Table mapping
            ToTable("MP_PLOTEO_FRECUENTE_ESPECIAL");

            //Primary Key
            HasKey(p => p.IdPloteoFrecuenteEspecial);

            //Properties
            Property(p => p.IdPloteoFrecuenteEspecial)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLOTEO_FRECUENTE_ESPECIAL");

            Property(p => p.IdPloteoFrecuente)
                .IsRequired()
                .HasColumnName("ID_PLOTEO_FRECUENTE");

            Property(p => p.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            Property(p => p.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(p => p.IdPlantillaViewport)
                .HasColumnName("ID_PLANTILLA_VIEWPORT");

            /*Property(p => p.Geometry)
                .HasColumnName("GEOMETRY");*/
            Ignore(p=>p.Geometry);

            Property(p => p.Orientacion)
                .IsRequired()
                .HasColumnName("ORIENTACION");

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
            
            Property(p => p.IdDistrito)
                .HasColumnName("ID_DISTRITO");

            Property(p => p.IdServicio)
                .HasColumnName("ID_SERVICIO");

            Property(p => p.Escala)
                .HasColumnName("ESCALA");
        }
    }
}
