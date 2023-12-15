using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class Hoja
    {
        public int IdHoja { get; set; }

        public string Nombre { get; set; }

        public int Alto_mm { get; set; }

        public int Ancho_mm { get; set; }

        public int? Ancho_Imagen_px { get; set; }

        [JsonIgnore]
        public ICollection<Plantilla> Plantillas { get; set; }
    }
}

