using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{
    public class UnidadPloteoPredefinido
    {
        public UnidadPloteoPredefinido()
        {
            this.OperacionesLayers = new Operaciones<Layer>();
            this.OperacionesTextos = new Operaciones<PlantillaTexto>();
            this.OperacionesEscalas = new Operaciones<PlantillaEscala>();
        }

        public OperationItem<Plantilla> OperacionPlantilla { get; set; }
        public OperationItem<PlantillaFondo> OperacionPlantillaFondo { get; set; }
        public Operaciones<Layer> OperacionesLayers { get; set; }
        public Operaciones<PlantillaTexto> OperacionesTextos { get; set; }
        public Operaciones<PlantillaEscala> OperacionesEscalas { get; set; }
    }
}
