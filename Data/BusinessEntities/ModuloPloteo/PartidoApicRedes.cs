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
    public class PartidoApicRedes
    {
        public long GId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Id { get; set; }
        public string Abrev { get; set; }
        public string Prestac { get; set; }

    }
}
