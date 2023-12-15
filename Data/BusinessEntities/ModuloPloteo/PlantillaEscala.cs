using System;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PlantillaEscala
    {
        public int IdPlantillaEscala { get; set; }

        public int IdPlantilla { get; set; }

        public int Escala { get; set; }

        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public long IdUsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? IdUsuarioBaja { get; set; }

        public DateTime? FechaBaja { get; set; }

        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

        public PlantillaEscala Clone()
        {
            return new PlantillaEscala
            {
                Escala = Escala
            };            
        }
    }
}
