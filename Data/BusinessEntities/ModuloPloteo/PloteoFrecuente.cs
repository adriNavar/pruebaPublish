using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PloteoFrecuente
    {
        public int IdPloteoFrecuente { get; set; }
        public string Nombre { get; set; }
        public int IdComponente { get; set; }
        public int IdAtributo { get; set; }
        public int IdPlantilla { get; set; }
        public int UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBaja { get; set;}
        public DateTime? FechaBaja { get; set;}
        public int IdServicio { get; set; }
        
        //public Componente Componente { get; set; }//ignore
    }
}
