using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class Resolucion
    {
        public int IdResolucion { get; set; }

        public int Valor { get; set; }


        public ICollection<PlantillaFondo> PlantillaFondos { get; set; } 
    }
}

