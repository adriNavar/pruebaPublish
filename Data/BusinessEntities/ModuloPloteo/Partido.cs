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
    public class Partido
    {
        public long IdPartido { get; set; }
        public string Nombre { get; set; }
        public int? IdProvincia { get; set; }
        public string ApicId { get; set; }
        public long? ApicGId { get; set; }

        public int? Prestac { get; set; }

        public string Abrev { get; set; }

    }
}
