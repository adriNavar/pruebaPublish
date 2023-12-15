using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class Norte
    {
        public int IdNorte { get; set; }

        public string Nombre { get; set; }

        #region Tipo de imagen

        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR ImagenFormat
        public string IType { get; set; }

        public ImageFormat ImagenFormat
        {
            get
            {
                switch (IType)
                {
                    case "PNG":
                        return ImageFormat.Png;
                    default:
                        return ImageFormat.Jpeg;
                }
            }

            set { IType = value.ToString(); }
        }

        #endregion

        #region Imagen

        //NO USAR DIRECTAMENTE ESTA PROPIEDAD, USAR Imagen
        public byte[] IBytes { get; set; }

        public string SBytes { get; set; }

        public Image Imagen
        {
            get
            {
                if (IBytes == null) return null;
                var memoryStream = new MemoryStream(IBytes);
                return Image.FromStream(memoryStream);
            }

            set
            {
                if (value == null) return;
                var image = new Bitmap(value);
                var memoryStream = new MemoryStream();
                image.Save(memoryStream, ImagenFormat);
                IBytes = memoryStream.ToArray();
            }
        }

        #endregion

        public int AltoPx { get; set; }

        public int AnchoPx { get; set; }

        [JsonIgnore]
        public ICollection<Plantilla> Plantillas { get; set; } 
    }
}
