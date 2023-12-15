using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class CoeficientesCalculados : IEntity
    {
        public Decimal? nCoeficiente1T { get; set; }
        public Decimal? nCoeficiente2T { get; set; }
        public Decimal? nCoeficiente3T { get; set; }
        public Decimal? nCoeficiente4T { get; set; }
        public Decimal? nCoeficiente1M { get; set; }
        public Decimal? nCoeficiente2M { get; set; }
        public Decimal? nCoeficiente3M { get; set; }
        public Decimal? nCoeficiente4M { get; set; }
        public long? v_idParcela { get; set; }
        public long? v_idMejora { get; set; }
        public long? widMejora { get; set; }
        public long? widParcela { get; set; }
       public Decimal? v_Coeficiente1t { get; set; }
        public Decimal? v_Coeficiente2t { get; set; }
        public Decimal? v_Coeficiente3t { get; set; }
        public Decimal? v_Coeficiente4t { get; set; }
   
    }
  
}

    
