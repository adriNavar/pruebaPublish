using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class InspectorTipoInspeccion : IEntity
    {
        public int InspectorTipoInspeccionID { get; set; }
        public long InspectorID { get; set; }
        public int TipoInspeccionID { get; set; }
    }
}
