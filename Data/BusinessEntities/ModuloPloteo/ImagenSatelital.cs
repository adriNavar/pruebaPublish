using System;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class ImagenSatelital
    {
        public int IdImagenSatelital { get; set; }
        public string Nombre { get; set; }
        public string Layers { get; set; }
        public string URL { get; set; }
        public string SRS { get; set; }
        public string Format { get; set; }
        public DateTime FechaImagen { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}