using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PlantillaCategoria
    {
        public short IdPlantillaCategoria { get; set; }

        public string Nombre { get; set; }

        [JsonIgnore]
        public ICollection<Plantilla> Plantillas { get; set; } 
    }
}
