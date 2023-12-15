using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class Layer
    {
        public int IdLayer { get; set; }

        public int IdPlantilla { get; set; }

        public long ComponenteId { get; set; }

        public string Nombre { get; set; }

        public short Categoria { get; set; }

        public short Orden { get; set; }

        public short FiltroGeografico { get; set; }

        public bool Contorno { get; set; }

        public string ContornoColor { get; set; }

        public double? ContornoGrosor { get; set; }

        public bool Relleno { get; set; }

        public string RellenoColor { get; set; }

        //Transparency: Para setear el alpha color. 255 es opaco, un nro entre 0 y 254 es semitransparente 
        public int? RellenoTransparencia { get; set; }

        public int PuntoRepresentacion { get; set; }

        public int? PuntoPredeterminado { get; set; }

        public string PuntoImagenNombre { get; set; }

        #region PuntoImagen
        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR PuntoImagen
        public Byte[] IBytes { get; set; }

        public Image PuntoImagen
        {
            get
            {
                if (IBytes == null) return null;
                using (var memoryStream = new MemoryStream(IBytes))
                {
                    return Image.FromStream(memoryStream);
                }
            }

            set
            {
                if (value == null) return;
                using (var image = new Bitmap(value))
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, PuntoImagenFormat);
                    IBytes = memoryStream.ToArray();
                }
            }
        }
        #endregion

        #region Tipo de imagen de PuntoImagen
        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR PuntoImagenFormat
        public string ITipo { get; set; }

        public ImageFormat PuntoImagenFormat
        {
            get
            {
                switch (ITipo)
                {
                    case "Png":
                        return ImageFormat.Png;
                    default:
                        return ImageFormat.Jpeg;
                }
            }

            set { ITipo = value.ToString(); }
        }
        #endregion

        public double? PuntoAlto { get; set; }

        public double? PuntoAncho { get; set; }

        public Pen Pen { get; set; }

        public Brush Brush { get; set; }

        public Brush FillBrush { get; set; }

        public bool Etiqueta { get; set; }

        public int? EtiquetaIdAtributo { get; set; }

        public string EtiquetaColor { get; set; }

        public string EtiquetaFuenteNombre { get; set; }

        public double? EtiquetaFuenteTamanio { get; set; }

        public string EtiquetaFuenteEstilo { get; set; }

        public bool EtiquetaMantieneOrientacion { get; set; }

        public double? EtiquetaEscalaReferencia { get; set; }

        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public long IdUsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? IdUsuarioBaja { get; set; }

        public DateTime? FechaBaja { get; set; }

        public long? FiltroIdAtributo { get; set; }

        public bool Pattern { get; set; }

        public double? PatternAncho { get; set; }

        public double? PatternAlto { get; set; }

        public double? PatternLineaAncho { get; set; }

        public string Dash { get; set; }

        public bool PuntoAtributoOrientacion { get; set; }

        public long? PuntoEscala { get; set; }

        public String CapaFiltro { get; set; }


        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

        public Componente Componente { get; set; }

        public Layer Clone()
        {
            return new Layer
            {
                ComponenteId = ComponenteId,
                Nombre = Nombre,
                Categoria = Categoria,
                Orden = Orden,
                FiltroGeografico = FiltroGeografico,
                Contorno = Contorno,
                ContornoColor = ContornoColor,
                ContornoGrosor = ContornoGrosor,
                PuntoRepresentacion = PuntoRepresentacion,
                PuntoPredeterminado = PuntoPredeterminado,
                PuntoAlto = PuntoAlto,
                PuntoAncho = PuntoAncho,
                Relleno = Relleno,
                RellenoColor = RellenoColor,
                RellenoTransparencia = RellenoTransparencia,
                Etiqueta = Etiqueta,
                EtiquetaIdAtributo = EtiquetaIdAtributo,
                EtiquetaColor = EtiquetaColor,
                EtiquetaFuenteNombre = EtiquetaFuenteNombre,
                EtiquetaFuenteTamanio = EtiquetaFuenteTamanio,
                EtiquetaFuenteEstilo = EtiquetaFuenteEstilo,
                EtiquetaMantieneOrientacion = EtiquetaMantieneOrientacion,
                FiltroIdAtributo = FiltroIdAtributo,
                Pattern = Pattern,
                PatternAncho = PatternAncho,
                PatternAlto = PatternAlto,
                PatternLineaAncho = PatternLineaAncho,
                PuntoAtributoOrientacion = PuntoAtributoOrientacion,
                PuntoEscala = PuntoEscala,
                CapaFiltro = CapaFiltro

            };
        }
    }
}
