using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class UbicacionRuteo
    {
        public long IdUbicacionRuteo { get; set; }
        public int Distrito { get; set; }
        public string Calle { get; set; }
        public int? Altura { get; set; }
        public string Interseccion { get; set; }
        public string Manzana { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
