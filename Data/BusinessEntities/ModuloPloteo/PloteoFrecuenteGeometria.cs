using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class PloteoFrecuenteGeometria
    {
        public int IdPloteoFrecuenteGeometria { get; set; }
        public int IdPloteoFrecuenteEspecial { get; set; }
        public int IdObjeto { get; set; }
        public string Tabla { get; set; }
        public string Esquema { get; set; }
        public int Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public int Usuario_Modificacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public int? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
    }
}
