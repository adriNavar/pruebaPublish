using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.Mappers.Oracle
{
    public class LayerMapper : EntityTypeConfiguration<Layer>
    {
        public LayerMapper()
        {
            //Table mapping
            ToTable("MP_LAYER");

            //Primary key
            HasKey(l => l.IdLayer);

            //Properties
            Property(l => l.IdLayer)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_LAYER");

            Property(l => l.IdPlantilla)
                .IsRequired()
                .HasColumnName("ID_PLANTILLA");

            Property(l => l.ComponenteId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");

            Property(l => l.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(l => l.Categoria)
                .IsRequired()
                .HasColumnName("CATEGORIA");

            Property(l => l.Orden)
                .IsRequired()
                .HasColumnName("ORDEN");

            Property(l => l.FiltroGeografico)
                .IsRequired()
                .HasColumnName("FILTRO_GEOGRAFICO");

            Property(l => l.Contorno)
                .IsRequired()
                .HasColumnName("CONTORNO");

            Property(l => l.ContornoColor)
                .IsOptional()
                .HasColumnName("CONTORNO_COLOR");

            Property(l => l.ContornoGrosor)
                .IsOptional()
                .HasColumnName("CONTORNO_GROSOR");

            Property(l => l.PuntoRepresentacion)
                .IsRequired()
                .HasColumnName("PUNTO_REPRESENTACION");

            Property(l => l.PuntoPredeterminado)
                .IsOptional()
                .HasColumnName("PUNTO_PREDETERMINADO");

            Property(l => l.PuntoImagenNombre)
                .IsOptional()
                .HasColumnName("PUNTO_IMAGEN_NOMBRE");

            Property(l => l.IBytes)
                .IsOptional()
                .HasColumnName("PUNTO_IMAGEN");

            Property(l => l.ITipo)
                .IsOptional()
                .HasColumnName("PUNTO_IMAGEN_TIPO");

            Property(l => l.PuntoAlto)
                .IsOptional()
                .HasColumnName("PUNTO_ALTO");

            Property(l => l.PuntoAncho)
                .IsOptional()
                .HasColumnName("PUNTO_ANCHO");

            Property(l => l.Relleno)
                .IsRequired()
                .HasColumnName("RELLENO");

            Property(l => l.RellenoColor)
                .IsOptional()
                .HasColumnName("RELLENO_COLOR");

            Property(l => l.RellenoTransparencia)
                .IsOptional()
                .HasColumnName("RELLENO_TRANSPARENCIA");

            Property(l => l.Etiqueta)
                .IsRequired()
                .HasColumnName("ETIQUETA");

            Property(l => l.EtiquetaIdAtributo)
                .IsOptional()
                .HasColumnName("ETIQUETA_ID_ATRIBUTO");

            Property(l => l.EtiquetaColor)
                .IsOptional()
                .HasColumnName("ETIQUETA_COLOR");

            Property(l => l.EtiquetaFuenteNombre)
                .IsOptional()
                .HasColumnName("ETIQUETA_FUENTE_NOMBRE");

            Property(l => l.EtiquetaFuenteTamanio)
                .IsOptional()
                .HasColumnName("ETIQUETA_FUENTE_TAMANIO");

            Property(l => l.EtiquetaFuenteEstilo)
                .IsOptional()
                .HasColumnName("ETIQUETA_FUENTE_ESTILO");

            Property(l => l.EtiquetaMantieneOrientacion)
                .IsOptional()
                .HasColumnName("ETIQUETA_MANTIENE_ORIENTACION");
            Property(l => l.EtiquetaEscalaReferencia)
                .IsOptional()
                .HasColumnName("ETIQUETA_ESCALA_REFERENCIA");

            Property(l => l.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            Property(l => l.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(l => l.IdUsuarioModificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            Property(l => l.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIFICACION");

            Property(l => l.IdUsuarioBaja)
                .IsOptional()
                .HasColumnName("USUARIO_BAJA");

            Property(l => l.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            Property(l => l.FiltroIdAtributo)
                .IsOptional()
                .HasColumnName("FILTRO_ID_ATRIBUTO");

            Property(l => l.Pattern)
                .IsRequired()
                .HasColumnName("PATTERN");

            Property(l => l.PatternAncho)
                .IsOptional()
                .HasColumnName("PATTERN_ANCHO");

            Property(l => l.PatternAlto)
                .IsOptional()
                .HasColumnName("PATTERN_ALTO");

            Property(l => l.PatternLineaAncho)
                .IsOptional()
                .HasColumnName("PATTERN_LINEA_ANCHO");

            Property(l => l.Dash)
                .IsOptional()
                .HasColumnName("DASH");

            Property(l => l.PuntoAtributoOrientacion)
                .IsOptional()
                .HasColumnName("PUNTO_ATRIBUTO_ORIENTACION");

            Property(l => l.PuntoEscala)
                .IsOptional()
                .HasColumnName("PUNTO_ESCALA");

            Property(l => l.CapaFiltro)
                .IsOptional()
                .HasColumnName("CAPA_FILTRO");

            //Non persistent fields
            Ignore(l => l.Pen);
            Ignore(l => l.Brush);
            Ignore(l => l.FillBrush);
            Ignore(l => l.PuntoImagen);
            Ignore(l => l.PuntoImagenFormat);

            //Relationship with Plantilla
            HasRequired(l => l.Plantilla)
                .WithMany(p => p.Layers)
                .HasForeignKey(l => l.IdPlantilla);

            //Relationship with Componente
            //HasRequired(l => l.Componente)
            //    .WithMany(c => c.Layers)
            //    .HasForeignKey(l => l.ComponenteId);
        }
    }
}
