
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class EjesVia : IEntity
    {
        public long Id_Eje_Via { get; set; }
        public long? Id_Via { get; set; }
        public long? Altura_Desde_D { get; set; }
        public long? Altura_Desde_I { get; set; }
        public long? Altura_Hasta_D { get; set; }
        public long? Altura_Hasta_I { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }
}

    
