using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaViewportMapper : EntityTypeConfiguration<PlantillaViewport>
    {
        public PlantillaViewportMapper()
        {
            //Table mapping
            ToTable("MP_PLANTILLA_VIEWPORT");

            //Primary Key
            HasKey(p => p.IdPlantillaViewport);

            //Properties
            Property(p => p.IdPlantillaViewport)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_VIEWPORT");

            Property(p => p.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(p => p.IdTipoViewPort)
                .IsRequired()
                .HasColumnName("ID_TIPO_VIEWPORT");

            Property(p => p.Orientacion)
                .IsRequired()
                .HasColumnName("ORIENTACION");

            Property(p => p.ImpresionXMin)
                .HasColumnName("IMPRESION_XMIN");

            Property(p => p.ImpresionYMin)
                .HasColumnName("IMPRESION_YMIN");

            Property(p => p.ImpresionXMax)
                .HasColumnName("IMPRESION_XMAX");

            Property(p => p.ImpresionYMax)
                .HasColumnName("IMPRESION_YMAX");

            Property(p => p.DistBuffer)
                .IsRequired()
                .HasColumnName("DIST_BUFFER");

            Property(p => p.OptimizarTamanioHoja)
                .IsRequired()
                .HasColumnName("OPTIMIZAR_TAMANIO_HOJA");

            Property(p => p.IdNorte)
                .HasColumnName("ID_NORTE");

            Property(p => p.NorteX)
                .HasColumnName("NORTE_X");

            Property(p => p.NorteY)
                .HasColumnName("NORTE_Y");

            Property(p => p.NorteAlto)
                .HasColumnName("NORTE_ALTO");

            Property(p => p.NorteAncho)
                .HasColumnName("NORTE_ANCHO");

            Property(p => p.IdFuncionAdicional)
                .HasColumnName("ID_FUNCION_ADICIONAL");

            //Property(p => p.IdUsuarioAlta)
            Property(p => p.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            //Property(p => p.IdUsuarioModificacion)
            Property(p => p.UsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            //Property(p => p.IdUsuarioBaja)
            Property(p => p.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            Property(p => p.Transparencia)
                .HasColumnName("TRANSPARENCIA");

            Property(p => p.Visibilidad)
                .IsRequired()
                .HasColumnName("VISIBILIDAD");
        }
    }
}
