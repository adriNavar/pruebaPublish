using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlantillaMapper: EntityTypeConfiguration<Plantilla>
    {
        public PlantillaMapper()
        {
            //Table mapping
            ToTable("MP_PLANTILLA");

            //Primary Key
            HasKey(p => p.IdPlantilla);

            //Properties
            Property(p => p.IdPlantilla)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(p => p.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(p => p.IdHoja)
                .IsRequired()
                .HasColumnName("ID_HOJA");

            Property(p => p.Orientacion)
                .IsRequired()
                .HasColumnName("ORIENTACION");

            Property(p => p.ImpresionXMin)
                .IsRequired()
                .HasColumnName("IMPRESION_XMIN");

            Property(p => p.ImpresionYMin)
                .IsRequired()
                .HasColumnName("IMPRESION_YMIN");

            Property(p => p.ImpresionXMax)
                .IsRequired()
                .HasColumnName("IMPRESION_XMAX");

            Property(p => p.ImpresionYMax)
                .IsRequired()
                .HasColumnName("IMPRESION_YMAX");

            Property(p => p.DistBuffer)
                .IsRequired()
                .HasColumnName("DIST_BUFFER");

            Property(p => p.OptimizarTamanioHoja)
                .IsRequired()
                .HasColumnName("OPTIMIZAR_TAMANIO_HOJA");

            Property(p => p.ReferenciaXMin)
                .IsRequired()
                .HasColumnName("REFERENCIA_XMIN");

            Property(p => p.ReferenciaYMin)
                .IsRequired()
                .HasColumnName("REFERENCIA_YMIN");

            Property(p => p.ReferenciaXMax)
                .IsRequired()
                .HasColumnName("REFERENCIA_XMAX");

            Property(p => p.ReferenciaYMax)
                .IsRequired()
                .HasColumnName("REFERENCIA_YMAX");

            Property(p => p.ReferenciaColor)
                .IsRequired()
                .HasColumnName("REFERENCIA_COLOR");

            Property(p => p.ReferenciaFuenteNombre)
                .IsRequired()
                .HasColumnName("REFERENCIA_FUENTE_NOMBRE");

            Property(p => p.ReferenciaFuenteTamanio)
                .IsRequired()
                .HasColumnName("REFERENCIA_FUENTE_TAMANIO");

            Property(p => p.ReferenciaFuenteEstilo)
                .IsRequired()
                .HasColumnName("REFERENCIA_FUENTE_ESTILO");

            Property(p => p.ReferenciaEspaciado)
                .IsRequired()
                .HasColumnName("REFERENCIA_ESPACIADO");

            Property(p => p.ReferenciaColumnas)
                .IsRequired()
                .HasColumnName("REFERENCIA_COLUMNAS");

            Property(p => p.IdNorte)
                .IsRequired()
                .HasColumnName("ID_NORTE");

            Property(p => p.NorteX)
                .IsRequired()
                .HasColumnName("NORTE_X");

            Property(p => p.NorteY)
                .IsRequired()
                .HasColumnName("NORTE_Y");

            Property(p => p.NorteAlto)
                .IsRequired()
                .HasColumnName("NORTE_ALTO");

            Property(p => p.NorteAncho)
                .IsRequired()
                .HasColumnName("NORTE_ANCHO");

            Property(p => p.IdFuncionAdicional)
                .IsOptional()
                .HasColumnName("ID_FUNCION_ADICIONAL");

            Property(p => p.Transparencia)
                .IsOptional()
                .HasColumnName("TRANSPARENCIA");

            Property(p => p.Visibilidad)
                .IsRequired()
                .HasColumnName("VISIBILIDAD");

            Property(p => p.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(p => p.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(p => p.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            Property(p => p.IdPlantillaCategoria)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA_CATEGORIA");

            /*Property(p => p.Rotacion)
                .HasColumnName("ROTACION");*/

            //Non persistent fields
            Ignore(p => p.Width);
            Ignore(p => p.Heigth);
            Ignore(p => p.WidthPts);
            Ignore(p => p.HeigthPts);
            Ignore(p => p.PPP);
            Ignore(p => p.Resolucion);
            Ignore(p => p.X_Centro);
            Ignore(p => p.Y_Centro);
            Ignore(p => p.X_Util);
            Ignore(p => p.Y_Util);

            Ignore(p => p.esViewport);
            Ignore(p => p.Geometry);
            //Ignore(p => p.Rotacion);
            
            //Relationship with Hoja
            HasRequired(p => p.Hoja)
                .WithMany(h => h.Plantillas)
                .HasForeignKey(p => p.IdHoja);

            //Relationship with Norte
            HasRequired(p => p.Norte)
                .WithMany(n => n.Plantillas)
                .HasForeignKey(p => p.IdNorte);

            //Relationship with FuncionAdicional
            HasOptional(p => p.FuncionAdicional)
                .WithMany(fa => fa.Plantillas)
                .HasForeignKey(p => p.IdFuncionAdicional);

            //Relationship with PlantillaCategoria
            HasRequired(p => p.PlantillaCategoria)
                .WithMany(pc => pc.Plantillas)
                .HasForeignKey(p => p.IdPlantillaCategoria);
        }
    }
}
