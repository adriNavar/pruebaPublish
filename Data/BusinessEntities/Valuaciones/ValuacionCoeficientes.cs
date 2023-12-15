
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValuacionCoeficientes : IEntity
    {
        public long ParcelaID { get; set; }
        public long UnidadTributariaID { get; set; }

        public Decimal CoeficienteTierra1 { get; set; }
        public Decimal CoeficienteTierra2 { get; set; }
        public Decimal CoeficienteTierra3 { get; set; }
        public Decimal CoeficienteTierra4 { get; set; }

        public Decimal CoeficienteMejora1 { get; set; }
        public Decimal CoeficienteMejora2 { get; set; }
        public Decimal CoeficienteMejora3 { get; set; }
        public Decimal CoeficienteMejora4 { get; set; }
    
    }
  
}

