using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class ObjetoResultadoDetalle : IEntity
    {
        public string GUID { get; set; }
        //public long Rangos { get; set; }
        public double RangoDesde { get; set; }
        public double RangoHasta { get; set; }
        public long Distribucion { get; set; }
        public List<Rango> Rangos { get; set; }
        public string NombreMT { get; set; }
        public int? GeometryType { get; set; }
        public long Transparencia { get; set; }
        public long IdUsuario { get; set; }
        public long ComponenteId { get; set; }
    }
}
