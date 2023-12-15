using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PloteoFrecuenteGeometriaMapper : EntityTypeConfiguration<PloteoFrecuenteGeometria>
    {
        public PloteoFrecuenteGeometriaMapper()
        {
            //Table mapping
            ToTable("MP_PLOTEO_FRECUENTE_GEOM");
            //Primary Key
            HasKey(p => p.IdPloteoFrecuenteGeometria);

            //Properties
            Property(p => p.IdPloteoFrecuenteGeometria)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLOTEO_FRECUENTE_GEOM");

            Property(p => p.IdPloteoFrecuenteEspecial)
                .IsRequired()
                .HasColumnName("ID_PLOTEO_FRECUENTE_ESPECIAL");

            Property(p => p.IdObjeto)
                .IsRequired()
                .HasColumnName("ID_OBJETO");

            Property(p => p.Tabla)
                .IsRequired()
                .HasColumnName("TABLA");

            Property(p => p.Esquema)
                .IsRequired()
                .HasColumnName("ESQUEMA");

            Property(p => p.Usuario_Alta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.Fecha_Alta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.Usuario_Modificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.Fecha_Modificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(p => p.Usuario_Baja)
            .HasColumnName("USUARIO_BAJA");

            Property(p => p.Fecha_Baja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
