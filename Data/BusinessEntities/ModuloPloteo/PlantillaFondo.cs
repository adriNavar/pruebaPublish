using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PlantillaFondo
    {
        public int IdPlantillaFondo { get; set; }

        public int IdPlantilla { get; set; }

        public int IdResolucion { get; set; }

        public int Alto_Px { get; set; }

        public int Ancho_Px { get; set; }

        public string ImagenNombre { get; set; }

        #region Tipo de imagen de fondo

        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR ImagenFormat
        //public string ITipo { get; set; }

        //public ImageFormat ImagenFormat
        //{
        //    get
        //    {
        //        switch (ITipo)
        //        {
        //            case "Png":
        //                return ImageFormat.Png;
        //            default:
        //                return ImageFormat.Jpeg;
        //        }
        //    }

        //    set { ITipo = value.ToString(); }
        //}

        #endregion

        #region Imagen de fondo

        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR ImagenImage
        public byte[] IBytes { get; set; }

        public Image ImagenImage
        {
            get;
            set;
            //get
            //{
            //    if (IBytes == null) return null;
            //    var memoryStream = new MemoryStream(IBytes);
            //    return Image.FromStream(memoryStream);
            //}

            //set
            //{
            //    if (value == null) return;
            //    var image = new Bitmap(value);
            //    var memoryStream = new MemoryStream();
            //    image.Save(memoryStream, ImagenFormat);
            //    IBytes = memoryStream.ToArray();
            //}
        }

        #endregion

        public MemoryStream PDFMemoryStream
        {
            get
            {
                if (IBytes == null) return null;
                var memoryStream = new MemoryStream(IBytes);
                return memoryStream;
            }

            set
            {
                if (value == null) return;
                IBytes = value.ToArray();
            }
        }

        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public long IdUsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? IdUsuarioBaja { get; set; }

        public DateTime? FechaBaja { get; set; }

        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

        public Resolucion Resolucion { get; set; }

        public PlantillaFondo Clone()
        {
            return new PlantillaFondo
            {
                IdResolucion = IdResolucion,
                Alto_Px = Alto_Px,
                Ancho_Px = Ancho_Px,
                ImagenNombre = ImagenNombre,
                Resolucion = Resolucion,
                IBytes = IBytes
            };
        }
    }
}
