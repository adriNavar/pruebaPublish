using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Common
{
    public class MERemitoParameters : IEntity
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public string SectorDestino { get; set; }
        public string SectorEmisor { get; set; }
        
        public int IdSectorOrigen { get; set; }
        public int IdSectorDestino { get; set; }
        public int[] MovimientosId { get; set; }

    }
}
