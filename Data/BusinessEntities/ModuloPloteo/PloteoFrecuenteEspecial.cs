using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PloteoFrecuenteEspecial
    {
        public int IdPloteoFrecuenteEspecial { get; set; }
        public int IdPloteoFrecuente { get; set; }
        public string Descripcion { get; set; }
        public int IdPlantilla { get; set; }
        public int? IdPlantillaViewport { get; set; }
        public string Geometry { get; set; }
        public int? Orientacion { get; set; }
        public int UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string IdDistrito{ get; set; }
        public int? IdServicio{ get; set; }
        public int? Escala { get; set; }
    }
}
