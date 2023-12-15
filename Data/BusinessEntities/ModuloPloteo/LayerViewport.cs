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
    public class LayerViewport
    {
        public long IdLayerViewport { get; set; }
        public long IdPlantillaViewport { get; set; }
        public int IdLayer { get; set; }
        public int UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        [JsonIgnore]
        public PlantillaViewport PlantillaViewport { get; set; }
        public Layer Layer { get; set; }
    }
}
